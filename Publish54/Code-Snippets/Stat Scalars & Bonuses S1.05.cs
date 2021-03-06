Stat Scalars & Bonuses version S1.05
Purpose: 
Correlate and Scale secondary stats (hits, mana, stamina) directly to primary stats (strength, intellect, dexterity)
Allow additional bonus to secondary stats
Additional breath attack controls
Additional paragon controls

Notes:
Scalars and Bonuses are ignored when a secondary stat has an arbitrary value declared.
System causes by default minimal deviation without further edits in creature files.

File: BaseCreature.cs
File Location: ...\Scripts\Mobiles\Normal\

Snippet Type: Section Addition
Suggested Location: Just below ForceNotoriety.
//GS
		// Stat multipliers
		public virtual long HitScale{ get{ return 1; } }				// Multiplies hitpoints from strength
		public virtual long StamScale{ get{ return 1; } }				// Multiplies stamina from dexterity
		public virtual long ManaScale{ get{ return 1; } }				// Multiplies manapoints from intelligence
		public virtual long ParagonHitScale{ get{ return 5; } }			// Multiplies hitpoints from strength for paragons
		public virtual long ParagonStamScale{ get{ return 1; } }		// Multiplies stamina from dexterity for paragons
		public virtual long ParagonManaScale{ get{ return 1; } }		// Multiplies manapoints from intelligence for paragons

		// Stat bonuses
		public virtual int HitBonus{ get{ return 0; } }					// Adds hitpoints to scaled total
		public virtual int StamBonus{ get{ return 0; } }				// Adds stamina to scaled total
		public virtual int ManaBonus{ get{ return 0; } }				// Adds manapoints to scaled total
		public virtual int ParagonHitBonus{ get{ return 0; } }			// Adds hitpoints to scaled total for paragons
		public virtual int ParagonStamBonus{ get{ return 0; } }			// Adds stamina to scaled total for paragons
		public virtual int ParagonManaBonus{ get{ return 0; } }			// Adds manapoints to scaled total for paragons

		// Breath Damage Controllers
		public virtual int BreathDamageMin{ get{ return 1; } }			// Minimum breath damage
		public virtual int BreathDamageMax{ get{ return 200; } }		// Maximum breath damage
		public virtual int BreathDamageBonus{ get{ return 0; } }		// Adds damage to breath attack total
		public virtual int ParagonBreathMin{ get{ return 1; } }			// Minimum breath damage for paragons
		public virtual int ParagonBreathMax{ get{ return 200; } }		// Maximum breath damage for paragons
		public virtual int ParagonBreathBonus{ get{ return 0; } }		// Adds damage to breath attack total for paragons
		public virtual long ParagonBreathScalar{ get{ return 1; } }		// Multiplies breath damage for paragons
//GS//

Snippet Type: Module Replacement
//GS
		[CommandProperty(AccessLevel.GameMaster)]
		public override int HitsMax
		{
			get
			{
				if (m_HitsMax > 0)
				{
					int value = m_HitsMax + GetStatOffset(StatType.Str);

					if (value < 1)
					{
						value = 1;
					}
					else if (value > 1000000)
					{
						value = 1000000;
					}

					return value;
				}
				
				if ( IsParagon )
				{
					return (int)(Str * ParagonHitScale + ParagonHitBonus);
				}

				return (int)(Str * HitScale + HitBonus);
			}
		}
//GS//

Snippet Type: Module Replacement
//GS
		[CommandProperty( AccessLevel.GameMaster )]
		public override int StamMax
		{
			get
			{
				if (m_StamMax > 0)
				{
					int value = m_StamMax + GetStatOffset(StatType.Dex);

					if (value < 1)
					{
						value = 1;
					}
					else if (value > 1000000)
					{
						value = 1000000;
					}

					return value;
				}

				if ( IsParagon )
				{
					return (int)(Dex * ParagonStamScale + ParagonStamBonus);
				}

				return (int)(Dex * StamScale + StamBonus);
			}
		}
//GS//

Snippet Type: Module Replacement
//GS
		[CommandProperty( AccessLevel.GameMaster )]
		public override int ManaMax
		{
			get
			{
				if (m_ManaMax > 0)
				{
					int value = m_ManaMax + GetStatOffset(StatType.Int);

					if (value < 1)
					{
						value = 1;
					}
					else if (value > 1000000)
					{
						value = 1000000;
					}

					return value;
				}

				if ( IsParagon )
				{
					return (int)(Int * ParagonManaScale + ParagonManaBonus);
				}

				return (int)(Int * ManaScale + ManaBonus);
			}
		}
//GS//

Snippet Type: Module Replacement
//GS
		[CommandProperty( AccessLevel.GameMaster )]
		public bool IsParagon
		{
			get{ return m_Paragon; }
			set
			{
				if ( m_Paragon == value )
				{
					return;
				}
				else if ( value )
				{
					XmlParagon.Convert(this);
				}
				else
				{
					XmlParagon.UnConvert(this);
				}

				m_Paragon = value;
				Hits = HitsMax;
				Mana = ManaMax;
				Stam = StamMax;
				InvalidateProperties();
			}
		}
//GS//

Snippet Type: Module Replacement
//GS
		public virtual int BreathComputeDamage()
		{
			int damage;

			if ( IsParagon )
			{
				damage = (int)(Hits * BreathDamageScalar * ParagonBreathScalar);

				if ( m_HitsMax > 0 )
				{
					damage = (int)(damage / XmlParagon.GetHitsBuff(this) + ParagonBreathBonus);
				}
				else
				{
					damage = (int)(damage * HitScale / ParagonHitScale + ParagonBreathBonus);
				}

				damage = Math.Max(damage, ParagonBreathMin);
				damage = Math.Min(damage, ParagonBreathMax);

				return damage;
			}

			damage = (int)(Hits * BreathDamageScalar + BreathDamageBonus);
			damage = Math.Max(damage, BreathDamageMin);
			damage = Math.Min(damage, BreathDamageMax);

			return damage;
		}
//GS//

Patchnotes:
S1.05 - 03/24/2017
Changed redundant 'else if Paragon' check nested in an 'if Paragon' check to 'else'
S1.04 - 11/16/2016
Removed NeverParagon, use native CanBeParagon
Removed OnBeforeSpawn module, remove NeverParagon references
Removed NeverParagon check in IsParagon (It aborts immediately before doing any Convert)
S1.03 - 11/03/2016
Added in-file information
S1.02 - 10/25/2016
Small formatting changes
S1.01 - 10/23/2016
Fixed paragons' breath damage to multiply by base hitscale, then divide by paragonhitscale, because paragonhitscale completely replaces hitscale for paragons
