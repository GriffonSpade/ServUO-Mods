Opposition List version S1.02
Purpose: Provide simpler, sleeker, and more versatile version of Opposition Group
Note: System causes by default no deviation without further edits in creature files.

File: BaseCreature.cs
File Location: ...\Scripts\Mobiles\Normal\

Snippet Type: Section Addition
Suggested Location: Just below ForceNotoriety.
//GS
		// Opposition List stuff
		public virtual OppositionType OppositionList{ get{ return OppositionType.None ; } }			// What opposition list am I in?
		public virtual bool OppositionPet{ get{ return false ; } }									// Do I attack tame members of my opposition?

		public enum OppositionType
		{
			None,
			Juka,
			Meer,
			Terathan,
			Ophidian,
			Savage,
			Orc,
			BlackSolen,
			RedSolen,
			Fey,
			Undead,
			Daemon,
			Dragon,
			Exodus,
			Elemental
		}

		public virtual bool OppositionListEnemy( Mobile m )
		{
			BaseCreature c = m as BaseCreature;
			
			if ( c == null )
			{
				return false;
			}

			if ( OppositionPet && c.ControlMaster != null )
			{
				return false;
			}
			
			switch ( OppositionList )
			{
				case OppositionType.Juka: return JukaEnemy(c.OppositionList);
				case OppositionType.Meer: return MeerEnemy(c.OppositionList);
				case OppositionType.Terathan: return TerathanEnemy(c.OppositionList);
				case OppositionType.Ophidian: return OphidianEnemy(c.OppositionList);
				case OppositionType.Savage: return SavageEnemy(c.OppositionList);
				case OppositionType.Orc: return OrcEnemy(c.OppositionList);
				case OppositionType.BlackSolen: return BlackSolenEnemy(c.OppositionList);
				case OppositionType.RedSolen: return RedSolenEnemy(c.OppositionList);
				case OppositionType.Fey: return FeyEnemy(c.OppositionList);
				case OppositionType.Undead: return UndeadEnemy(c.OppositionList);
				case OppositionType.Daemon: return DaemonEnemy(c.OppositionList);
				case OppositionType.Dragon: return DragonEnemy(c.OppositionList);
				case OppositionType.Exodus: return ExodusEnemy(c.OppositionList);
				case OppositionType.Elemental: return ElementalEnemy(c.OppositionList);
				default: return false;
			}
		}
		
		private bool JukaEnemy( OppositionType egroup )
		{
			switch ( egroup )
			{
				case OppositionType.Meer: return true;
				default: return false;
			}
		}

		private bool MeerEnemy( OppositionType egroup )
		{
			switch ( egroup )
			{
				case OppositionType.Juka: return true;
				default: return false;
			}
		}

		private bool TerathanEnemy( OppositionType egroup )
		{
			switch ( egroup )
			{
				case OppositionType.Ophidian: return true;
				default: return false;
			}
		}

		private bool OphidianEnemy( OppositionType egroup )
		{
			switch ( egroup )
			{
				case OppositionType.Terathan: return true;
				default: return false;
			}
		}

		private bool SavageEnemy( OppositionType egroup )
		{
			switch ( egroup )
			{
				case OppositionType.Orc: return true;
				default: return false;
			}
		}

		private bool OrcEnemy( OppositionType egroup )
		{
			switch ( egroup )
			{
				case OppositionType.Savage: return true;
				default: return false;
			}
		}

		private bool BlackSolenEnemy( OppositionType egroup )
		{
			switch ( egroup )
			{
				case OppositionType.RedSolen: return true;
				default: return false;
			}
		}

		private bool RedSolenEnemy( OppositionType egroup )
		{
			switch ( egroup )
			{
				case OppositionType.BlackSolen: return true;
				default: return false;
			}
		}

		private bool FeyEnemy( OppositionType egroup )
		{
			switch ( egroup )
			{
				case OppositionType.Undead: return true;
				default: return false;
			}
		}

		private bool UndeadEnemy( OppositionType egroup )
		{
			switch ( egroup )
			{
				case OppositionType.Fey: return true;
				default: return false;
			}
		}

		private bool DaemonEnemy( OppositionType egroup )
		{
			switch ( egroup )
			{
				default: return false;
			}
		}

		private bool DragonEnemy( OppositionType egroup )
		{
			switch ( egroup )
			{
				default: return false;
			}
		}

		private bool ExodusEnemy( OppositionType egroup )
		{
			switch ( egroup )
			{
				default: return false;
			}
		}

		private bool ElementalEnemy( OppositionType egroup )
		{
			switch ( egroup )
			{
				default: return false;
			}
		}
//GS//

Snippet Type: Module Replacement
//GS
		public virtual bool IsFriend( Mobile m )
		{
			if (OppositionListEnemy(m))
			{
				return false;
			}

			OppositionGroup g = OppositionGroup;

			if (g != null && g.IsEnemy(this, m))
			{
				return false;
			}

			if (!(m is BaseCreature))
			{
				return false;
			}

			BaseCreature c = (BaseCreature)m;

			if (m_iTeam != c.m_iTeam)
			{
				return false
			}
/*
			if (c.Combatant == this)
			{
				return false;
			}
*/	
			return ((m_bSummoned || m_bControlled) == (c.m_bSummoned || c.m_bControlled));
		}
//GS//

Snippet Type: Module Replacement
//GS
		public virtual bool IsEnemy(Mobile m)
		{
			XmlIsEnemy a = (XmlIsEnemy)XmlAttach.FindAttachment(this, typeof(XmlIsEnemy));

			if (a != null)
			{
				return a.IsEnemy(m);
			}

			if (OppositionListEnemy(m))
			{
				return true;
			}

			OppositionGroup g = OppositionGroup;

			if (g != null && g.IsEnemy(this, m))
			{
				return true;
			}

			if (m is BaseGuard)
			{
				return false;
			}

			// Faction Allied Players/Pets are not my enemies
			if (GetFactionAllegiance(m) == Allegiance.Ally)
			{
				return false;
			}

			Ethic ourEthic = EthicAllegiance;
			Player pl = Ethics.Player.Find(m, true);

			// Ethic Allied Players/Pets are not my enemies
			if (pl != null && pl.IsShielded && (ourEthic == null || ourEthic == pl.Ethic))
			{
				return false;
			}

			if (m is PlayerMobile && ((PlayerMobile)m).HonorActive)
			{
				return false;
			}

			if (TransformationSpellHelper.UnderTransformation(m, typeof(EtherealVoyageSpell)))
			{
				return false;
			}

			if (!(m is BaseCreature) || m is MilitiaFighter)
			{
				return true;
			}

			BaseCreature c = (BaseCreature)m;
			BaseCreature t = this;

			// Summons should have same rules as their master
			if (c.Summoned && c.SummonMaster != null && c.SummonMaster is BaseCreature)
			{
				c = c.SummonMaster as BaseCreature;
			}

			if (t.Summoned && t.SummonMaster != null && t.SummonMaster is BaseCreature)
			{
				t = t.SummonMaster as BaseCreature;
			}

			// Creatures on other teams are my enemies
			if (t.m_iTeam != c.m_iTeam)
			{
				return true;
			}
/*
			// Creatures attacking me are my enemies
			if (c.Combatant == this)
			{
				return true;
			}
*/
			// If I'm summoned/controlled and they aren't summoned/controlled, they are my enemy
			// If I'm not summoned/controlled and they are summoned/controlled, they are my enemy
			return ((t.m_bSummoned || t.m_bControlled) != (c.m_bSummoned || c.m_bControlled));
		}
//GS//

Patchnotes:
vS1.03 - 03/24/2017
Updated IsFriend and IsEnemy modules
vS1.02 - 11/03/2016
Added In-File notes
vS1.01 - 10/26/2016
fixed elemental OT to use elemental module, rather than exodus
