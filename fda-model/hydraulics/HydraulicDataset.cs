using RasMapperLib;
using RasMapperLib.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;

namespace fda_hydro.hydraulics
{
    // Class needs to
    // get hydraulic data for a point and save it into an object
    // assign values from the point shapefile to the object
    // check which element of a polygon shapefile each structure belongs to, and save that to the object
    //
    //Also need to handle both grids and hdf. 
    //

    public class HydraulicDataset
    {
        public IList<HydraulicProfile> HydraulicProfiles { get; set; }
        public HydraulicDataset(IList<HydraulicProfile> profiles)
        {
        }
       
    }
}

