﻿using RasMapperLib;
using System;
using System.Collections.Generic;
using System.Linq;


namespace structures
{
    //TODO: Figure out how to set Occupany Type Set
    public class Inventory
    {
        private List<Structure> _structures;
        private List<OccupancyType> _Occtypes;
        private List<string> _damageCategories;
        private List<int> _impactAreaIDs; 
        public List<Structure> Structures { get; set; }
        public List<int> ImpactAreas
        {
            get { return _impactAreaIDs; }
        }
        public List<string> DamageCategories
        {
            get { return _damageCategories; }
        }

        public static T TryGet<T>(object value, T defaultValue = default)
            where T : struct
        {
            if (value == null)
                return defaultValue;
            else if (value == DBNull.Value)
                return defaultValue;
            else
            {
                var retn = value as T?;
                if (retn.HasValue)
                    return retn.Value;
                else
                    return defaultValue;
            }
        }
        public static T TryGetObj<T>(object value, T defaultValue = default)
            where T : class
        {
            if (value == null)
                return defaultValue;
            else if (value == DBNull.Value)
                return defaultValue;
            else
            {
                var retn = value as T;
                if (retn != null)
                    return retn;
                else
                    return defaultValue;
            }
        }


        /// <summary>
        /// Constructor to create a SI from a shapefile. Gonna need to do this from database potentially as well
        /// </summary>
        /// <param name="pointShapefilePath"></param>
        public Inventory(string pointShapefilePath, string impactAreaShapefilePath)
        {
            string[] columnHeaders = new string[] { "fd_id", "found_ht", "ground_elv", "val_struct", "val_cont", "val_vehic", "st_damcat", "occtype", "cbfips", "ff_elev" };
            PointFeatureLayer structureInventory = new PointFeatureLayer("Structure_Inventory", pointShapefilePath);
            PointMs pointMs = new PointMs(structureInventory.Points().Select(p => p.PointM()));
            Structures = new List<Structure>();
            for (int i = 0; i < structureInventory.FeatureCount(); i++)
            {
                //TODO: check behavior when header does not exist
                //TODO: Check RAS Mapper behavior on pulling rows from shapefiles. 
                // if cell is empty, we'll get dbnull 
                // if column header doesn't exist. Null - test this. 
                PointM point = pointMs[i];
                var row = structureInventory.FeatureRow(i);
                int fid = TryGet<int>(row["fd_id"]);
                double found_ht = TryGet<double>(row["found_ht"],-9999);
                double ground_elv = TryGet<double>(row["ground_elv"]);
                double val_struct = TryGet<double>(row["val_struct"]);
                double val_cont = TryGet<double>(row["val_cont"]);
                double val_vehic = TryGet<double>(row["val_vehic"]);
                double val_other = TryGet<double>(row["val_vehic"]);
                string st_damcat = TryGetObj<string>(row["st_damcat"]);
                string occtype = TryGetObj<string>(row["occtype"]);
                string cbfips = TryGetObj<string>(row["cbfips"]);
                double ff_elev = TryGet<double>(row["ff_elev"]);
                if(row["ff_elev"] == System.DBNull.Value)
                {
                    ff_elev = ground_elv + found_ht;
                }
                int impactAreaID = GetImpactAreaID(point, impactAreaShapefilePath);
                Structures.Add(new Structure(fid, point, found_ht, val_struct, val_cont, val_vehic, val_other, st_damcat, occtype, impactAreaID, cbfips));
            }
            GetUniqueImpactAreas();
            GetUniqueDamageCatagories();
        }
        // Will need a constructor/load from Database ; 


        public Inventory(List<Structure> structures, List<OccupancyType> occTypes)
        {
            _structures = structures;
            _Occtypes = occTypes;
            GetUniqueImpactAreas();
            GetUniqueDamageCatagories();
        }
        private void GetUniqueImpactAreas()
        {
            List<int> impactAreas = new List<int>();
            foreach (var structure in Structures)
            {
                if (!impactAreas.Contains(structure.ImpactAreaID))
                {
                    impactAreas.Add(structure.ImpactAreaID);
                }
            }
            _impactAreaIDs = impactAreas;
        }
        /// <summary>
        /// Loops through entire inventory and reports back a list of all the unique damage catagories associated with the structures
        /// </summary>
        /// <returns></returns>
        internal void GetUniqueDamageCatagories()
        {
            List<string> damageCatagories = new List<string>();
            foreach (Structure structure in Structures)
            {
                if (damageCatagories.Contains(structure.DamageCatagory))
                {
                    continue;
                }
                else
                {
                    damageCatagories.Add(structure.DamageCatagory);
                }
            }
            _damageCategories = damageCatagories;
        }
        public Inventory GetInventoryTrimmmedToPolygon(Polygon impactArea)
        {
            List<Structure> filteredStructureList = new List<Structure>();

            foreach (Structure structure in _structures)
            {
                if (impactArea.Contains(structure.Point))
                {
                    filteredStructureList.Add(structure);
                }
            }
            return new Inventory(filteredStructureList, _Occtypes);
        }

        public PointMs GetPointMs()
        {
            PointMs points = new PointMs();
            foreach (Structure structure in _structures)
            {
                points.Add(structure.Point);
            }
            return points;
        }

        private int GetImpactAreaID(PointM point, string polygonShapefilePath)
        {
            PolygonFeatureLayer polygonFeatureLayer = new PolygonFeatureLayer("impactAreas", polygonShapefilePath);
            List<Polygon> polygons = polygonFeatureLayer.Polygons().ToList();
            var polygonsList = polygons.ToList();
            for (int i = 0; i < polygonsList.Count; i++)
            {
                if (polygons[i].Contains(point))
                {
                    var row = polygonFeatureLayer.FeatureRow(i);
                    return (int)row["FID"];
                }
            }
            return -9999;
        }

        public DeterministicInventory Sample(int seed)
        {
            Random random = new Random(seed);

            List<DeterministicStructure> inventorySample = new List<DeterministicStructure>();
            foreach (Structure structure in _structures)
            {
                foreach (OccupancyType occupancyType in _Occtypes)
                {
                    if (structure.DamageCatagory.Equals(occupancyType.DamageCategory))
                    {
                        if (structure.OccTypeName.Equals(occupancyType.Name))
                        {
                            inventorySample.Add(structure.Sample(random.Next(), occupancyType));
                            break;
                        }
                    }
                }
                //it is possible that if an occupancy type doesnt exist a structure wont get added...
            }
            return new DeterministicInventory(inventorySample, _impactAreaIDs, _damageCategories);
        }
    }
}
