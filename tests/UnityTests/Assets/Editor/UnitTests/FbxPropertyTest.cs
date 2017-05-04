// ***********************************************************************
// Copyright (c) 2017 Unity Technologies. All rights reserved.
//
// Licensed under the ##LICENSENAME##.
// See LICENSE.md file in the project root for full license information.
// ***********************************************************************
using NUnit.Framework;
using FbxSdk;

namespace UnitTests
{
    public class FbxPropertyTest
    {

#if ENABLE_COVERAGE_TEST
        [Test]
        public void TestCoverage() {
            // Alphabetical list, with FbxProperty at the top.
            CoverageTester.TestCoverage(typeof(FbxProperty), this.GetType());
            CoverageTester.TestCoverage(typeof(FbxPropertyBool), this.GetType());
            CoverageTester.TestCoverage(typeof(FbxPropertyDouble), this.GetType());
            CoverageTester.TestCoverage(typeof(FbxPropertyDouble3), this.GetType());
            CoverageTester.TestCoverage(typeof(FbxPropertyEBlendMode), this.GetType());
            CoverageTester.TestCoverage(typeof(FbxPropertyEWrapMode), this.GetType());
            CoverageTester.TestCoverage(typeof(FbxPropertyString), this.GetType());
        }
#endif

        [Test]
        public void TestEquality() {
            using(var manager = FbxManager.Create()) {
                // FbxProperty
                var node = FbxNode.Create(manager, "node");
                var prop1 = FbxProperty.Create(node, Globals.FbxBoolDT, "bool1");
                var prop2 = FbxProperty.Create(node, Globals.FbxBoolDT, "bool2");
                var prop1copy = node.FindProperty("bool1");
                EqualityTester<FbxProperty>.TestEquality(prop1, prop2, prop1copy);

                // FbxPropertyT<bool>
                var vis1 = node.VisibilityInheritance;
                var vis2 = FbxNode.Create(manager, "node2").VisibilityInheritance;
                var vis1copy = vis1; // TODO: node.FindProperty("Visibility Inheritance"); -- but that has a different proxy type
                EqualityTester<FbxPropertyBool>.TestEquality(vis1, vis2, vis1copy);

                // FbxPropertyT<double>
                var lambert = FbxSurfaceLambert.Create(manager, "lambert");
                var emissiveCopy = lambert.EmissiveFactor; // TODO: lambert.FindProperty("EmissiveFactor");
                EqualityTester<FbxPropertyDouble>.TestEquality(lambert.EmissiveFactor, lambert.AmbientFactor, emissiveCopy);

                // FbxPropertyT<FbxDouble3>
                var lclTranslationCopy = node.LclTranslation; // TODO: node.FindProperty("Lcl Translation");
                EqualityTester<FbxPropertyDouble3>.TestEquality(node.LclTranslation, node.LclRotation, lclTranslationCopy);

                // FbxPropertyT<> for FbxTexture enums
                var tex1 = FbxTexture.Create(manager, "tex1");
                var tex2 = FbxTexture.Create(manager, "tex2");
                var blendCopy = tex1.CurrentTextureBlendMode; // TODO: tex1.FindProperty(...)
                EqualityTester<FbxPropertyEBlendMode>.TestEquality(tex1.CurrentTextureBlendMode, tex2.CurrentTextureBlendMode, blendCopy);
                var wrapCopy = tex1.WrapModeU; // TODO: tex1.FindProperty(...)
                EqualityTester<FbxPropertyEWrapMode>.TestEquality(tex1.WrapModeU, tex2.WrapModeU, wrapCopy);

                // FbxPropertyT<string>
                var impl = FbxImplementation.Create(manager, "impl");
                var renderAPIcopy = impl.RenderAPI; // TODO: impl.FindProperty("RenderAPI");
                EqualityTester<FbxPropertyString>.TestEquality(impl.RenderAPI, impl.RenderAPIVersion, renderAPIcopy);
            }
        }

        // tests that should work for any subclass of FbxProperty
        public static void GenericPropertyTests<T>(T property, FbxObject parent, string propertyName, FbxDataType dataType) where T:FbxProperty{
            Assert.IsTrue(property.IsValid());
            Assert.AreEqual(dataType, property.GetPropertyDataType());
            Assert.AreEqual(propertyName, property.GetName());
            Assert.AreEqual(propertyName, property.ToString());
            Assert.AreEqual(propertyName, property.GetHierarchicalName());
            Assert.AreEqual(propertyName, property.GetLabel(true));
            property.SetLabel("label");
            Assert.AreEqual("label", property.GetLabel());
            Assert.AreEqual(parent, property.GetFbxObject());
            Assert.AreEqual(property.GetFbxObject(), parent); // test it both ways just in case equals is busted

            // test GetCurve() (make sure it doesn't crash)
            FbxAnimLayer layer = FbxAnimLayer.Create(parent, "layer");

            FbxAnimCurve curve = property.GetCurve (layer, null);
            Assert.IsNull (curve);

            // should create curve if none found
            // TODO: returns a null curve, maybe because there is no channel defined?
            curve = property.GetCurve(layer, null, true);

            FbxAnimCurve curve2 = property.GetCurve (layer, null);
            Assert.AreEqual (curve, curve2);

            // test GetCurve() null args
            Assert.That (() => { property.GetCurve(null, ""); }, Throws.Exception.TypeOf<System.NullReferenceException>());
            property.GetCurve(layer, null); // doesn't throw an exception, gets handled in C++

            // verify this in the future: will dispose destroy?
            property.Dispose();
        }

        [Test]
        public void BasicTests ()
        {
            using (var manager = FbxManager.Create()) {
                // FbxPropertyT<FbxBool> example: VisibilityInheritance on a node
                var node = FbxNode.Create(manager, "node");
                GenericPropertyTests<FbxPropertyBool> (node.VisibilityInheritance, node, "Visibility Inheritance", Globals.FbxVisibilityInheritanceDT);

                var property = node.VisibilityInheritance;
                property.Set(false);
                Assert.AreEqual(false, property.Get());

                Assert.IsTrue(property.Set(5.0f));
                Assert.AreEqual(true, property.Get());
            }

            using(var manager = FbxManager.Create()) {
                // FbxPropertyT<FbxDouble> example: several of them on a Lambert shader
                var obj = FbxSurfaceLambert.Create(manager, "lambert");
                GenericPropertyTests<FbxPropertyDouble> (obj.EmissiveFactor, obj, "EmissiveFactor", Globals.FbxDoubleDT);

                var property = obj.EmissiveFactor;
                property.Set(5.0); // bool Set<float> is not accessible here!
                Assert.AreEqual(5.0, property.Get());
            }

            using(var manager = FbxManager.Create()) {
                // FbxPropertyT<Double3> example: the LclTranslation on a node
                var node = FbxNode.Create(manager, "node");
                GenericPropertyTests<FbxPropertyDouble3> (node.LclTranslation, node, "Lcl Translation", Globals.FbxLocalTranslationDT);

                var property = node.LclTranslation;
                property.Set(new FbxDouble3(1,2,3));
                Assert.AreEqual(new FbxDouble3(1, 2, 3), property.Get());

                Assert.IsTrue(property.Set(5.0f));
                Assert.AreEqual(new FbxDouble3(5.0f), property.Get());
            }

            using (var manager = FbxManager.Create()) {
                // FbxPropertyT for FbxTexture enums EBlendMode and EWrapMode
                var tex = FbxTexture.Create(manager, "tex");

                FbxPropertyTest.GenericPropertyTests(tex.CurrentTextureBlendMode, tex, "CurrentTextureBlendMode", Globals.FbxEnumDT);
                tex.CurrentTextureBlendMode.Set(FbxTexture.EBlendMode.eAdditive);
                Assert.AreEqual(FbxTexture.EBlendMode.eAdditive, tex.CurrentTextureBlendMode.Get());
                tex.CurrentTextureBlendMode.Set(5.0f);
                Assert.AreEqual(5, (int)tex.CurrentTextureBlendMode.Get());

                FbxPropertyTest.GenericPropertyTests(tex.WrapModeU, tex, "WrapModeU", Globals.FbxEnumDT);
                tex.WrapModeU.Set(FbxTexture.EWrapMode.eClamp);
                Assert.AreEqual(FbxTexture.EWrapMode.eClamp, tex.WrapModeU.Get());
                tex.WrapModeU.Set(5.0f);
                Assert.AreEqual(5, (int)tex.WrapModeU.Get());
            }

            using (var manager = FbxManager.Create()) {
                // FbxPropertyT<FbxString> example: the description of a shader implementation
                var impl = FbxImplementation.Create(manager, "name");
                GenericPropertyTests<FbxPropertyString> (impl.RenderAPI, impl, "RenderAPI", Globals.FbxStringDT);

                var property = impl.RenderAPI;
                property.Set("a value");
                Assert.AreEqual("a value", property.Get());

                Assert.IsTrue(property.Set(5.0f));
                Assert.AreEqual("5.000000", property.Get());

                property.Dispose();
                using(var prop2 = impl.RenderAPI) { }
            }

            using (var manager = FbxManager.Create()) {
                // Test all the create and destroy operations
                FbxProperty root, child;
                var obj = FbxObject.Create(manager, "obj");

                Assert.IsNotNull(FbxProperty.Create(obj, Globals.FbxStringDT, "a"));
                Assert.IsNotNull(FbxProperty.Create(obj, Globals.FbxStringDT, "b", "label"));
                Assert.IsNotNull(FbxProperty.Create(obj, Globals.FbxStringDT, "c", "label", false));
                bool didFind;
                Assert.IsNotNull(FbxProperty.Create(obj, Globals.FbxStringDT, "c", "label", true, out didFind));
                Assert.IsTrue(didFind);

                root = FbxProperty.Create(obj, Globals.FbxCompoundDT, "root");

                child = FbxProperty.Create(root, Globals.FbxStringDT, "a");
                Assert.IsNotNull(child);
                Assert.IsNotNull(FbxProperty.Create(root, Globals.FbxStringDT, "b", "label"));
                Assert.IsNotNull(FbxProperty.Create(root, Globals.FbxStringDT, "c", "label", false));
                Assert.IsNotNull(FbxProperty.Create(root, Globals.FbxStringDT, "c", "label", true, out didFind));
                Assert.IsTrue(didFind);

                child.Destroy();

                root.DestroyChildren();
                Assert.IsNotNull(FbxProperty.Create(root, Globals.FbxStringDT, "c", "label", true, out didFind));
                Assert.IsFalse(didFind);

                root.DestroyRecursively();
            }
        }
    }
}