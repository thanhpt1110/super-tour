﻿using Super_Tour.Ultis.Api_Address;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Super_Tour.Model
{
    public class PACKAGE
    {
        [Key]
        public int Id_Package { get; set; }
        public string Name_Package { get; set; }
        public int Id_Type_Package { get; set; }
        public string Id_Province { get; set; }
        public string Id_District { get; set; }
        public string Image_Package { get; set; }
        public string Description_Package { get; set; }
        [NotMapped] 
        public string ProvinceName { get; set; }
        [NotMapped]
        public string DistrictName { get; set; }
        public decimal Price { get; set; }
        [ForeignKey("Id_Type_Package")]
        public virtual TYPE_PACKAGE TYPEPACKAGE { get; set; }
        public virtual ICollection<TOUR_DETAILS> TOUR_DETAILs
        {
            get; set;
        }
    }
}
