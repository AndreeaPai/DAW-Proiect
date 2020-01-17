using AutoMapper;
using ArtShop.Data.Entities;
using ArtShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ArtShop.Data
{
    public class ArtShopMappingProfile : Profile //e inauntru mapping namespace
    {
        public ArtShopMappingProfile()
        {
            /*am creat un map intre cele 2
             se va uita la proprietatilor celor 2
             si va incerca sa potriveasca prop cu prop*/
            CreateMap<Order, OrderModel>()
                .ForMember(o => o.OrderId, ex => ex.MapFrom(o=> o.Id))/*cand cauti pt OrderId,mapeaza din sursa((o=> o.Id)) lui Id*/
                .ReverseMap();//ia info care o ai despre Map (de sus) si fao sa mearga si pt opus

            CreateMap<OrderItem, OrderItemModel>()
                .ReverseMap();
        }
    }
}
