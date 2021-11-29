using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RelationshipsReworked.Util
{   
    // This class is intended to dictate the pawn's sexuality from traits.
    public enum PawnSexualityTrait
    {
        GAY,
        DEFAULT, // This value is here to let us know that the pawn's sexuality is not overwritten by traits. It's Hetero by default but can be changed in mod settings.
        BISEXUAL,
        ASEXUAL
    }
}
