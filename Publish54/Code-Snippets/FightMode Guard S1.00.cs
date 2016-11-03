Purpose: adds Guard FightMode that can be added to creatures. 
It behaves as the Aggressor FightMode, with the exception that it will attack criminals and murderers.
Current Status: Unreleased
Reason: Behavior is unsatisfactory because players can attack guards without being flagged criminal.
	
File: BaseCreature.cs
File Location: ...\Scripts\Mobiles\Normal\

Snippet Type: Module Replacement
//GS
    public enum FightMode
    {
	None, // Never focus on others
	Aggressor, // Only attack aggressors
	Strongest, // Attack the strongest
	Weakest, // Attack the weakest
	Closest, // Attack the closest
	Evil, // Only attack aggressor -or- negative karma
	Good, // Only attack aggressor -or- positive karma
	Guard // Only attack aggressor -or- criminal/murderer
    }
//GS//

File: BaseAI.cs
File Location: ServUO-master\Scripts\Mobiles\AI\

Snippet Type: Module Replacement
//GS
		public virtual bool AcquireFocusMob(int iRange, FightMode acqType, bool bPlayerOnly, bool bFacFriend, bool bFacFoe)
		{
			if (m_Mobile.Deleted)
			{
				return false;
			}

			if (m_Mobile.BardProvoked)
			{
				if (m_Mobile.BardTarget == null || m_Mobile.BardTarget.Deleted)
				{
					m_Mobile.FocusMob = null;
					return false;
				}
				else
				{
					m_Mobile.FocusMob = m_Mobile.BardTarget;
					return (m_Mobile.FocusMob != null);
				}
			}
			else if (m_Mobile.Controlled)
			{
				if (m_Mobile.ControlTarget == null || m_Mobile.ControlTarget.Deleted || m_Mobile.ControlTarget.Hidden ||
					!m_Mobile.ControlTarget.Alive || m_Mobile.ControlTarget.IsDeadBondedPet ||
					!m_Mobile.InRange(m_Mobile.ControlTarget, m_Mobile.RangePerception * 2))
				{
					if (m_Mobile.ControlTarget != null && m_Mobile.ControlTarget != m_Mobile.ControlMaster)
					{
						m_Mobile.ControlTarget = null;
					}

					m_Mobile.FocusMob = null;
					return false;
				}
				else
				{
					m_Mobile.FocusMob = m_Mobile.ControlTarget;
					return (m_Mobile.FocusMob != null);
				}
			}

			if (m_Mobile.ConstantFocus != null)
			{
				m_Mobile.DebugSay("Acquired my constant focus");
				m_Mobile.FocusMob = m_Mobile.ConstantFocus;
				return true;
			}

			if (acqType == FightMode.None)
			{
				m_Mobile.FocusMob = null;
				return false;
			}

			if (acqType == FightMode.Aggressor && m_Mobile.Aggressors.Count == 0 && m_Mobile.Aggressed.Count == 0 &&
				m_Mobile.FactionAllegiance == null && m_Mobile.EthicAllegiance == null)
			{
				m_Mobile.FocusMob = null;
				return false;
			}

			if (m_Mobile.NextReacquireTime > Core.TickCount)
			{
				m_Mobile.FocusMob = null;
				return false;
			}

			m_Mobile.NextReacquireTime = Core.TickCount + (int)m_Mobile.ReacquireDelay.TotalMilliseconds;

			m_Mobile.DebugSay("Acquiring...");

			Map map = m_Mobile.Map;

			if (map != null)
			{
				Mobile newFocusMob = null;
				double val = double.MinValue;
				double theirVal;

				var eable = map.GetMobilesInRange(m_Mobile.Location, iRange);

				foreach (Mobile m in eable)
				{
					if (m.Deleted || m.Blessed)
					{
						continue;
					}

					// Let's not target ourselves...
					if (m == m_Mobile || m is BaseFamiliar)
					{
						continue;
					}

					// Dead targets are invalid.
					if (!m.Alive || m.IsDeadBondedPet)
					{
						continue;
					}

					// Staff members cannot be targeted.
					if (m.IsStaff())
					{
						continue;
					}

					// Does it have to be a player?
					if (bPlayerOnly && !m.Player)
					{
						continue;
					}

					// Can't acquire a target we can't see.
					if (!m_Mobile.CanSee(m))
					{
						continue;
					}

					if (m_Mobile.Summoned && m_Mobile.SummonMaster != null)
					{
						// If this is a summon, it can't target its controller.
						if (m == m_Mobile.SummonMaster)
							continue;

						// It also must abide by harmful spell rules if the master is a player.
						if (m_Mobile.SummonMaster is PlayerMobile && !Server.Spells.SpellHelper.ValidIndirectTarget(m_Mobile.SummonMaster, m))
							continue;

						// Players animated creatures cannot attack other players directly.
						if (m is PlayerMobile && m_Mobile.IsAnimatedDead && m_Mobile.SummonMaster is PlayerMobile)
							continue;
					}

					// If we only want faction friends
					if (bFacFriend && !bFacFoe)
					{
						// Ignore anyone who's not a friend
						if (!m_Mobile.IsFriend(m))
						{
							continue;
						}
					}
					// Don't ignore friends we want to and can help
					else if (!bFacFriend || !m_Mobile.IsFriend(m))
					{
						// Ignore anyone we can't hurt
						if (!m_Mobile.CanBeHarmful(m, false))
						{
							continue;
						}

						// Don't ignore hostile mobiles
						if (!IsHostile(m))
						{
							// Ignore anyone if we don't want enemies
							if (!bFacFoe)
							{
								continue;
							}

							//Ignore anyone under EtherealVoyage
							if (TransformationSpellHelper.UnderTransformation(m, typeof(EtherealVoyageSpell)))
							{
								continue;
							}

							// Ignore players with activated honor
							if (m is PlayerMobile && ((PlayerMobile)m).HonorActive && !(m_Mobile.Combatant == m))
							{
								continue;
							}

							// Xmlspawner faction check
							// Ignore mob faction ranked players, more highly more often
							//if (!Server.Engines.XmlSpawner2.XmlMobFactions.CheckAcquire(this.m_Mobile, m))
							//continue;

							// We want a faction/ethic enemy
							bool bValid = (m_Mobile.GetFactionAllegiance(m) == BaseCreature.Allegiance.Enemy ||
										  m_Mobile.GetEthicAllegiance(m) == BaseCreature.Allegiance.Enemy);

							BaseCreature c = m as BaseCreature;

							// We want a special FightMode enemy
							if (!bValid)
							{
								// We want a karma enemy
								if (acqType == FightMode.Evil)
								{
									if (c != null && c.Controlled && c.ControlMaster != null)
									{
										bValid = (c.ControlMaster.Karma < 0);
									}
									else
									{
										bValid = (m.Karma < 0);
									}
								}
								// We want a karma enemy
								else if (acqType == FightMode.Good)
								{
									if (c != null && c.Controlled && c.ControlMaster != null)
									{
										bValid = (c.ControlMaster.Karma > 0);
									}
									else
									{
										bValid = (m.Karma > 0);
									}
								}
								// We want a criminal
								else if (acqType == FightMode.Guard)
								{
									if (c != null)
									{
										// Ignore other FightMode Guards
										if (c.FightMode == FightMode.Guard)
											continue;

										if (c.AlwaysMurderer)
											bValid = true;
									}

									if (m.Criminal || m.Kills >= 5)
									{
										bValid = true;
									}
								}
							}

							// Don't ignore valid targets
							if (!bValid)
							{
								// Ignore anyone if we are a Passive FightMode
								if (acqType == FightMode.Good || acqType == FightMode.Evil || acqType == FightMode.Aggressor || acqType == FightMode.Guard)
								{
									continue;
								}
								// Ignore anyone if we are an Uncontrolled Summon
								else if (c != null && (c.Summoned))
								{
									continue;
								}
								// We want an enemy (We are an Aggressive FightMode)
								else if (m_Mobile.IsEnemy(m))
								{
									bValid = true;
								}
								// Ignore anyone else
								else
								{
									continue;
								}
							}
						}
					}

					theirVal = m_Mobile.GetFightModeRanking(m, acqType, bPlayerOnly);

					if (theirVal > val && m_Mobile.InLOS(m))
					{
						newFocusMob = m;
						val = theirVal;
					}
				}

				eable.Free();

				m_Mobile.FocusMob = newFocusMob;
			}

			return (m_Mobile.FocusMob != null);
		}
//GS//
