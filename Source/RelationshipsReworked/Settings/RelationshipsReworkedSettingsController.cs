using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using UnityEngine;

namespace RelationshipsReworked.Settings
{
    class RelationshipsReworkedSettingsController : Mod
    {
        public RelationshipsReworkedSettingsController(ModContentPack contentPack) : base(contentPack)
        {
            GetSettings<RelationshipsReworkedSettings>();
        }

        public override string SettingsCategory()
        {
            return "Relationships Reworked";
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            RelationshipsReworkedSettings.DoSettingsWindowContents(inRect);
        }
    }
}
