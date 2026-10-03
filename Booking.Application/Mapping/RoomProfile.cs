using AutoMapper;
using Booking.Application.DTOs.Hotels;
using Booking.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Application.Mapping;

public class RoomProfile:Profile
{
    public RoomProfile() {

        CreateMap<Room, RoomDto>();
        CreateMap<RoomCreateDto, Room>();

    }
    
}
