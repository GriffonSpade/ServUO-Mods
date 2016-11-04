AI True Speed version S1.00
Purpose: Removes arbitrary creature speed alterations and creates speed controls
Note: System causes changes by default

File: BaseCreature.cs
File Location: ...\Scripts\Mobiles\Normal\

Snippet Type: Section Addition
Suggested Location: Just below ForceNotoriety.
//GS
		// AI Delay modifiers
		public virtual bool XmlOverrideDelay{ get{ return true; } }		// XML overrides DamageDelayFactor instead of modify
		public virtual double PassiveMoveDelay{ get{ return 0.5; } }	// Delay Added to passive creature AI delay
		public virtual double ControlMoveDelay{ get{ return 0.0; } }	// Delay Added to controlled creature AI delay
		public virtual double MinimumMoveDelay{ get{ return 0.05; } }	// AI delay floor
		public virtual double DamageDelayFactor{ get{ return 0.2; } }	// Damage Delay scalar
		public virtual double DamageDelayStart{ get{ return 0.5; } }	// Health percentage which is considered full health for Damage Delay Factor
//GS//

File: BaseAI.cs
File Location: ...\Scripts\Mobiles\AI\

Snippet Type: Module Replacement
//GS
		public double TransformMoveDelay(double delay)
		{
			BaseCreature c = m_Mobile as BaseCreature;

			if (!c.Controlled)
			{
				delay += (delay == c.PassiveSpeed) ? c.PassiveMoveDelay : 0;
			}
			else
			{
				delay += c.ControlMoveDelay;
			}

			double speedfactor = c.DamageDelayFactor;

			XmlValue a = (XmlValue)XmlAttach.FindAttachment(c, typeof(XmlValue), "DamagedSpeedFactor");

			if (a != null)
			{
				if (c.XmlOverrideDelay)
				{
					speedfactor = a.Value / 100.0;
				}
				else
				{
					speedfactor *= a.Value / 100.0;
				}
			}

			if (!c.IsDeadPet && (c.ReduceSpeedWithDamage || c.IsSubdued))
			{
				double offset = (double)c.Hits / (c.HitsMax * c.DamageDelayStart);

				if (offset < 0.0)
				{
					offset = 0.0;
				}
				else if (offset > 1.0)
				{
					offset = 1.0;
				}

				offset = 1.0 - offset;

				delay += (offset * speedfactor);
			}

			delay = Math.Max(delay, c.MinimumMoveDelay);

			if (double.IsNaN(delay))
			{
				using (StreamWriter op = new StreamWriter("nan_transform.txt", true))
				{
					op.WriteLine(
						String.Format(
							"NaN in TransformMoveDelay: {0}, {1}, {2}, {3}",
							DateTime.UtcNow,
							GetType(),
							m_Mobile == null ? "null" : c.GetType().ToString(),
							c.HitsMax));
				}

				return 1.0;
			}

			return delay;
		}
//GS//
