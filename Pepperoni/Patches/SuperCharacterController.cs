using MonoMod;
using UnityEngine;

#pragma warning disable CS0108, CS0626, CS0114, CS0169, CS0649
namespace Pepperoni.Patches
{
    [MonoModPatch("global::SuperCharacterController")]
    public class SuperCharacterController : global::SuperCharacterController
    {
        [MonoModIgnore] private Quaternion lastGroundRotation;
        [MonoModIgnore] private Vector3 lastGroundPosition;
        [MonoModIgnore] private Vector3 lastGroundOffset;
        [MonoModIgnore] public bool clampToMovingGround;


        public class SuperGround
        {
            [MonoModPublic]
            public LayerMask walkable;
        }


        public Vector3 LastGroundPos { get { return lastGroundPosition; } set { lastGroundPosition = value; } }
        public Quaternion LastGroundRot { get { return lastGroundRotation; } set { lastGroundRotation = value; } }
        public Vector3 LastGroundOffset { get { return lastGroundOffset; } set { lastGroundOffset = value; } }
        public bool ClampMovGround { get { return clampToMovingGround; } set { clampToMovingGround = value; } }

    }
}
