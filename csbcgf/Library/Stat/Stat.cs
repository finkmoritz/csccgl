﻿using Newtonsoft.Json;

namespace csbcgf
{
    public class Stat : IStat
    {
        public const int GlobalMin = int.MinValue;
        public const int GlobalMax = int.MaxValue;

        [JsonProperty]
        protected int value;

        [JsonProperty]
        protected int baseValue;

        protected Stat() { }

        /// <summary>
        /// Represents a Card's property.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="baseValue"></param>
        public Stat(int value, int baseValue)
        {
            this.baseValue = baseValue;
            this.value = value;
        }

        [JsonIgnore]
        public virtual int Value
        {
            get => value;
            set => this.value = Math.Max(GlobalMin, Math.Min(GlobalMax, value));
        }

        [JsonIgnore]
        public virtual int BaseValue
        {
            get => baseValue;
            set => baseValue = Math.Max(GlobalMin, Math.Min(GlobalMax, value));
        }
    }
}
