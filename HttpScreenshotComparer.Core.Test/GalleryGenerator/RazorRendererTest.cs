using System;
using System.Collections.Generic;
using System.Text;
using FluentAssertions;
using HttpScreenshotComparer.Core.GalleryGenerator;
using Moq;
using Xunit;

namespace HttpScreenshotComparer.Core.Test.GalleryGenerator
{
    public class RazorRendererTest
    {
        public class MyClass
        {
            public int Prop1 { get; set; }
            public string Prop2 { get; set; }
        }

        [Fact]
        public void TestRazorRenderWithEmptyTemplatePath()
        {
            //Assign
            var myClass = new MyClass()
            {
                Prop1 = 1,
                Prop2 = "Test"
            };
            var razorRenderer = new RazorRenderer();

            //Act
            var ex = Assert.Throws<ArgumentNullException>(() => razorRenderer.Render(string.Empty, myClass));
            
            //Assert
            ex.Should().NotBeNull("An exception should be throw");
        }

        [Fact]
        public void TestRazorRenderWithNullModel()
        {
            //Assign            
            var razorRenderer = new RazorRenderer();

            //Act
            var ex = Assert.Throws<ArgumentNullException>(() => razorRenderer.Render<MyClass>("coucou", null));

            //Assert
            ex.Should().NotBeNull("An exception should be throw");
        }

        [Fact]
        public void TestRazorRenderWithFilledTemplate()
        {
            //Assign           
            string[] templateValue = {
                @"@model HttpScreenshotComparer.Core.Test.GalleryGenerator.RazorRendererTest.MyClass
                Prop1: @Model.Prop1, Prop2: @Model.Prop2"
                                    };

            var razorRendererMoq = new Mock<RazorRenderer>(); //To Allow the debug
            razorRendererMoq.Setup(rr => rr.GetViewContent(It.IsAny<string>()))
                            .Returns(() => templateValue);

            var razorRenderer = razorRendererMoq.Object;

            var myClass = new MyClass()
            {
                Prop1 = 1,
                Prop2 = "Test"
            };

            //Act
            var result = razorRenderer.Render("coucou", myClass);

            //Assert
            result.Trim().Should().Be($"Prop1: {myClass.Prop1}, Prop2: {myClass.Prop2}");
        }

        [Fact]
        public void TestRazorRenderTwice()
        {
            //Assign           
            string[] templateValue = {
                @"@model HttpScreenshotComparer.Core.Test.GalleryGenerator.RazorRendererTest.MyClass
                Prop1: @Model.Prop1, Prop2: @Model.Prop2"
                                    };

            var razorRendererMoq = new Mock<RazorRenderer>(); //To Allow the debug
            razorRendererMoq.Setup(rr => rr.GetViewContent(It.IsAny<string>()))
                            .Returns(() => templateValue);

            var razorRenderer = razorRendererMoq.Object;

            var myClass = new MyClass()
            {
                Prop1 = 1,
                Prop2 = "Test"
            };
            var myClass2 = new MyClass()
            {
                Prop1 = 2,
                Prop2 = "Test2"
            };

            //Act
            var result = razorRenderer.Render("coucou", myClass);
            var result2 = razorRenderer.Render("coucou", myClass2);

            //Assert
            result.Trim().Should().Be($"Prop1: {myClass.Prop1}, Prop2: {myClass.Prop2}");
            result2.Trim().Should().Be($"Prop1: {myClass2.Prop1}, Prop2: {myClass2.Prop2}");
        }

    }
}
