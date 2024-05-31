using AutoMapper;
using Old = ASPWeb.Models;
using New = ControllerDomain.Entities;
using SkudTransferApi.Contexts;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace SkudWebApplication
{
    public class AppMappingProfile : Profile
    {
        public AppMappingProfile() 
        {
            CreateMap<Old.Controllers, New.Controller>()
                .ForMember(x => x.Id, x => x.Ignore())
                .ForMember(x => x.ControllerLocationId, x => x.Ignore())
                .ForMember(x => x.ControllerLocation, x => x.Ignore())
                .ForMember(x => x.FwVer, x => x.MapFrom(y => y.fw))
                .ForMember(x => x.ComFwVer, x => x.MapFrom(y => y.conn_fw))
                .ForMember(x => x.IpAddress, x => x.MapFrom(y => y.controller_ip));
            CreateMap<New.Controller, New.Controller>()
                .ForMember(x => x.Id, x => x.Ignore())
                .ForMember(x => x.ControllerLocationId, x => x.Ignore())
                .ForMember(x => x.ControllerLocation, x => x.Ignore());

            CreateMap<Old.Groups, New.WorkerGroup>()
                .ForMember(x => x.Id, x => x.Ignore())
                .ForMember(x => x.Name, x => x.MapFrom(y => y.group))
                .ForMember(x => x.Arch, x => x.MapFrom(y => false))
                .ForMember(x => x.Workers, x => x.Ignore());
            CreateMap<New.WorkerGroup, New.WorkerGroup>()
                .ForMember(x => x.Id, x => x.Ignore())
                .ForMember(x => x.Workers, x => x.Ignore());

            var date = new DateTime();
            CreateMap<Old.Workers, New.Worker>()
                .ForMember(x => x.Id, x => x.Ignore())
                .ForMember(x => x.LastName, x => x.MapFrom(y => y.LastName ?? string.Empty))
                .ForMember(x => x.FirstName, x => x.MapFrom(y => y.FirstName ?? string.Empty))
                .ForMember(x => x.FatherName, x => x.MapFrom(y => y.FatherName ?? string.Empty))
                .ForMember(x => x.Position, x => x.MapFrom(y => y.position ?? string.Empty))
                .ForMember(x => x.AccessMethodId, x => x.MapFrom(y => y.personalCheck))
                .ForMember(x => x.Comment, x => x.MapFrom(y => y.comment ?? string.Empty))
                .ForMember(x => x.DateBlock, x => x.Ignore())
                .ForMember(x => x.ImagePath, x => x.MapFrom(y => y.Image ?? string.Empty))
                .ForMember(x => x.DisanId, x => x.MapFrom(y => y.worker))
                .ForMember(x => x.Arch, x => x.MapFrom(y => false))
                .ForMember(x => x.Group, x => x.Ignore());
            CreateMap<New.Worker, New.Worker>()
                .ForMember(x => x.Id, x => x.Ignore());

            CreateMap<Old.Cards, New.Card>()
                .ForMember(x => x.Id, x => x.Ignore())
                .ForMember(x => x.WorkerId, x => x.MapFrom(y => y.workerId))
                .ForMember(x => x.CardNumb, x => x.MapFrom(y => y.card))
                .ForMember(x => x.CardNumb16, x => x.MapFrom(y => y.card16))
                .ForMember(x => x.Arch, x => x.MapFrom(y => false));
            CreateMap<New.Controller, New.Controller>()
                .ForMember(x => x.Id, x => x.Ignore());

            CreateMap<Old.Events, New.Event>()
                .ForMember(x => x.Id, x => x.Ignore())
                .ForMember(x => x.WorkerId, x => x.Ignore())
                .ForMember(x => x.Worker, x => x.Ignore())
                .ForMember(x => x.ControllerLocation, x => x.Ignore())
                .ForMember(x => x.ControllerLocationId, x => x.Ignore())
                .ForMember(x => x.EventTypeId, x => x.MapFrom(x => x.Event))
                .ForMember(x => x.Create, x => x.MapFrom(x => x.dateTime.ToUniversalTime()))
                .ForMember(x => x.Flag, x => x.MapFrom(x => x.flag));
        }
    }
}
