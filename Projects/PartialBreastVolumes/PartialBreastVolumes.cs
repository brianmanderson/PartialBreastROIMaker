using System;
using System.Linq;
using System.Text;
using System.Windows;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;

// TODO: Replace the following version attributes by creating AssemblyInfo.cs. You can do this in the properties of the Visual Studio project.
[assembly: AssemblyVersion("1.0.0.1")]
[assembly: AssemblyFileVersion("1.0.0.1")]
[assembly: AssemblyInformationalVersion("1.0")]

// TODO: Uncomment the following line if the script requires write access.
[assembly: ESAPIScript(IsWriteable = true)]

namespace VMS.TPS
{
    public class Script
    {
        public Script()
        {
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public void Execute(ScriptContext context /*, System.Windows.Window window, ScriptEnvironment environment*/)
        {
            Patient patient = context.Patient;
            patient.BeginModifications();

            StructureSet ss = context.StructureSet;
            make_skin(ss);
            make_ptv(ss);
            make_ptv_eval(ss);
            // TODO : Add here the code that is called when the script is launched from Eclipse.
        }
        public void make_skin(StructureSet ss)
        {
            Structure body = ss.Structures.First(st => st.Id == "BODY");
            if (ss.CanAddStructure("ORGAN", "Skin"))
            {
                Structure skin_structure = ss.AddStructure("ORGAN", "Skin");
                skin_structure.SegmentVolume = body.Margin(marginInMM: 5).Sub(body.SegmentVolume);
            }
            else
            {
                Structure skin_structure = ss.Structures.First(st => st.Id == "Skin");
                skin_structure.SegmentVolume = body.Margin(marginInMM: 5).Sub(body.SegmentVolume);
            }
        }
        public void make_ptv(StructureSet ss)
        {
            Structure cavity = ss.Structures.First(st => st.Id == "Cavity");
            Structure chest_wall = ss.Structures.First(st => st.Id == "Chest Wall");
            Structure body = ss.Structures.First(st => st.Id == "BODY");
            SegmentVolume body_shell = body.Sub(body.Margin(marginInMM: -5));
            if (ss.CanAddStructure("PTV", "PTV"))
            {
                Structure ptv = ss.AddStructure("PTV", "PTV");
                ptv.SegmentVolume = cavity.Margin(marginInMM: 10);  // Ptv is cavity expanded by 10 mm
                ptv.SegmentVolume = ptv.SegmentVolume.And(body.SegmentVolume); // Within the body
                ptv.SegmentVolume = ptv.SegmentVolume.Sub(body_shell); // Excluding the body reduced by 5 mm
                ptv.SegmentVolume = ptv.SegmentVolume.Sub(chest_wall.SegmentVolume); // And outside of chest wall
            }
            else
            {
                Structure ptv = ss.Structures.First(st => st.Id == "PTV");
                ptv.SegmentVolume = cavity.Margin(marginInMM: 10);  // Ptv is cavity expanded by 10 mm
                ptv.SegmentVolume = ptv.SegmentVolume.And(body.SegmentVolume); // Within the body
                ptv.SegmentVolume = ptv.SegmentVolume.Sub(body_shell); // Excluding the body reduced by 5 mm
                ptv.SegmentVolume = ptv.SegmentVolume.Sub(chest_wall.SegmentVolume); // And outside of chest wall
            }
        }
        public void make_ptv_eval(StructureSet ss)
        {
            Structure cavity = ss.Structures.First(st => st.Id == "Cavity");
            Structure ptv = ss.Structures.First(st => st.Id == "PTV");
            if (ss.CanAddStructure("PTV", "PTVeval"))
            {
                Structure ptveval = ss.AddStructure("PTV", "PTVeval");
                ptveval.SegmentVolume = ptv.SegmentVolume.Sub(cavity.SegmentVolume);
            }
            else
            {
                Structure ptveval = ss.Structures.First(st => st.Id == "PTVeval");
                ptveval.SegmentVolume = ptv.SegmentVolume.Sub(cavity.SegmentVolume);
            }
        }
    }
}
