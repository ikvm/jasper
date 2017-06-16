﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using Jasper.Bus.Runtime.Serializers;
using Newtonsoft.Json;
using Shouldly;
using Xunit;

namespace Jasper.Testing.Bus.Runtime.Serializers
{
    public class JsonMessageSerializerTester
    {
        public JsonMessageSerializerTester()
        {
            var settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All,
                PreserveReferencesHandling = PreserveReferencesHandling.Objects
            };

            _serializer = new JsonMessageSerializer(settings);
        }

        private readonly JsonMessageSerializer _serializer;

        [Fact]
        public void can_round_trip()
        {
            var employee1 = new Employee
            {
                Name = "Austin"
            };

            var stream = new MemoryStream();
            _serializer.Serialize(employee1, stream);

            stream.Position = 0;

            var employee2 = _serializer.Deserialize(stream).ShouldBeOfType<Employee>();
            employee1.ShouldBe(employee2);
        }

        [Fact]
        public void can_round_trip_circular_dependencies()
        {
            var employee = new Employee
            {
                Name = "Jim"
            };
            var boss = new Employee
            {
                Name = "Boss",
                Subordinates = {employee}
            };
            employee.Supervisor = boss;


            var stream = new MemoryStream();
            _serializer.Serialize(new List<Employee> {boss, employee}, stream);

            stream.Position = 0;

            var employees = _serializer.Deserialize(stream).ShouldBeOfType<List<Employee>>();
            var newBoss = employees.First();
            var newEmployee = employees.Last();

            newBoss.ShouldBe(boss);
            newEmployee.ShouldBe(employee);
            //Same object reference
            newEmployee.Supervisor.ShouldBeTheSameAs(newBoss);
        }


    }


    public class Employee
    {
        public Employee()
        {
            Subordinates = new List<Employee>();
        }

        public string Name { get; set; }
        public Employee Supervisor { get; set; }
        public List<Employee> Subordinates { get; set; }

        protected bool Equals(Employee other)
        {
            return string.Equals(Name, other.Name) && Equals(Supervisor, other.Supervisor);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Employee) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Name != null ? Name.GetHashCode() : 0;
                hashCode = (hashCode * 397) ^ (Supervisor != null ? Supervisor.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}