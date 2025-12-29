using NUnit.Framework; // Le framework de test d'Unity
using UnityEngine;
using MyLib.Core.Utilities.Extensions; // On teste nos extensions

namespace MyLib.Core.Tests.Editor
{
    public class VectorExtensionsTests
    {
        // Teste si la méthode WithX remplace bien la valeur X sans toucher aux autres.
        [Test]
        public void WithX_ChangesXComponent_AndKeepsYZ()
        {
            // 1. Arrange (Préparation)
            Vector3 original = new Vector3(10f, 20f, 30f);
            float newX = 50f;

            // 2. Act (Action)
            Vector3 result = original.WithX(newX);

            // 3. Assert (Vérification)
            Assert.AreEqual(newX, result.x); // Le X doit être 50
            Assert.AreEqual(original.y, result.y); // Le Y ne doit pas avoir bougé
            Assert.AreEqual(original.z, result.z); // Le Z ne doit pas avoir bougé
        }

        // Teste si la méthode WithY fonctionne.
        [Test]
        public void WithY_ChangesYComponent()
        {
            Vector3 original = new Vector3(1, 2, 3);
            Vector3 result = original.WithY(5);

            Assert.AreEqual(5, result.y);
            Assert.AreEqual(1, result.x);
        }

        // Teste si l'ajout (AddX) fonctionne correctement.
        [Test]
        public void AddX_IncrementsXComponent()
        {
            Vector3 original = new Vector3(10, 0, 0);
            Vector3 result = original.AddX(5);

            Assert.AreEqual(15, result.x);
        }
    }
}