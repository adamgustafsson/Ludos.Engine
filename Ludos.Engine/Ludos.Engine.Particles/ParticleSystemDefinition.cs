namespace Ludos.Engine.Particles
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;

    public struct ParticleSystemDefinition
    {
        public int Amount;
        public Type ParticleType;
        public List<Vector2> Positions;
        public float Scale;
        public bool DoRepeat;
        public ParticleSystemType Type;
    }
}
