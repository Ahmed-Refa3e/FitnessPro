﻿using Core.DTOs.PostDTO;
using Core.Entities.GymEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities.PostEntities
{
    public class GymPost:Post
    {
        public int GymId { get; set; }
        public Gym Gym {  get; set; }
    }
}
