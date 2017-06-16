﻿using System;
using System.Reflection;
using Baseline.Reflection;
using Jasper.Bus.Model;
using Jasper.Bus.Runtime.Cascading;
using Shouldly;
using Xunit;

namespace Jasper.Testing.Bus.Model
{
    public class HandlerCallTester
    {
        [Fact]
        public void throws_chunks_if_you_try_to_use_a_method_with_no_inputs()
        {
            Exception<ArgumentOutOfRangeException>.ShouldBeThrownBy(
                () => { HandlerCall.For<ITargetHandler>(x => x.ZeroInZeroOut()); });
        }

        [Fact]
        public void could_handle()
        {
            var handler1 = HandlerCall.For<SomeHandler>(x => x.Interface(null));
            var handler2 = HandlerCall.For<SomeHandler>(x => x.BaseClass(null));

            ShouldBeBooleanExtensions.ShouldBeTrue(handler1.CouldHandleOtherMessageType(typeof (Input1)));
            ShouldBeBooleanExtensions.ShouldBeTrue(handler2.CouldHandleOtherMessageType(typeof (Input1)));

            ShouldBeBooleanExtensions.ShouldBeFalse(handler1.CouldHandleOtherMessageType(typeof (Input2)));
            ShouldBeBooleanExtensions.ShouldBeFalse(handler1.CouldHandleOtherMessageType(typeof (Input2)));
        }

        [Fact]
        public void could_handle_is_false_for_its_own_input_type()
        {
            var handler = HandlerCall.For<ITargetHandler>(x => x.OneInOneOut(null));
            ShouldBeBooleanExtensions.ShouldBeFalse(handler.CouldHandleOtherMessageType(typeof (Input)));
        }


        [Fact]
        public void handler_call_should_not_match_property_setters()
        {
            var handlerType = typeof (ITargetHandler);
            var property = handlerType.GetTypeInfo().GetProperty("Message");
            var method = property.GetSetMethod();
            ShouldBeBooleanExtensions.ShouldBeFalse(HandlerCall.IsCandidate(method));
        }


        [Fact]
        public void is_candidate()
        {
            ShouldBeBooleanExtensions.ShouldBeFalse(HandlerCall.IsCandidate(ReflectionHelper.GetMethod<ITargetHandler>(x => x.ZeroInZeroOut())));
            ShouldBeBooleanExtensions.ShouldBeTrue(HandlerCall.IsCandidate(ReflectionHelper.GetMethod<ITargetHandler>(x => x.OneInOneOut(null))));
            ShouldBeBooleanExtensions.ShouldBeTrue(HandlerCall.IsCandidate(ReflectionHelper.GetMethod<ITargetHandler>(x => x.OneInZeroOut(null))));
            ShouldBeBooleanExtensions.ShouldBeFalse(HandlerCall.IsCandidate(ReflectionHelper.GetMethod<ITargetHandler>(x => x.ManyIn(null, null))));
            ShouldBeBooleanExtensions.ShouldBeFalse(HandlerCall.IsCandidate(ReflectionHelper.GetMethod<ITargetHandler>(x => x.ReturnsValueType(null))));
        }

        [Fact]
        public void is_candidate_allows_interface_return_types()
        {
            ShouldBeBooleanExtensions.ShouldBeTrue(HandlerCall.IsCandidate(ReflectionHelper.GetMethod<ITargetHandler>(x => x.ReturnsInterface(null))));
        }





        public interface ITargetHandler
        {
            string Message { get; set; }
            Output OneInOneOut(Input input);
            void OneInZeroOut(Input input);
            object OneInManyOut(Input input);
            void ZeroInZeroOut();

            void ManyIn(Input i1, Input i2);

            IImmediateContinuation ReturnsInterface(Input input);
            bool ReturnsValueType(Input input);
        }

        public class Input
        {
        }

        public class DifferentInput
        {

        }

        public class SpecialInput : Input
        {

        }

        public class Output
        {
        }

        public interface IInput { }
        public abstract class InputBase { }
        public class Input1 : InputBase, IInput { }
        public class Input2 { }

        public class SomeHandler
        {
            public void Interface(IInput input) { }
            public void BaseClass(InputBase input) { }
        }

    }
}