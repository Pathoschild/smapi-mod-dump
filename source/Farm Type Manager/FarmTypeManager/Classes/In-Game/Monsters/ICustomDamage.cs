using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FarmTypeManager.Monsters
{
    /// <summary>An interface for subclasses of Monster that use a custom property to override hardcoded DamageToFarmer values.</summary>
    interface ICustomDamage
    {
        int CustomDamage { get; set; } //when applicable, this should be modified *instead* of DamageToFarmer
    }
}
