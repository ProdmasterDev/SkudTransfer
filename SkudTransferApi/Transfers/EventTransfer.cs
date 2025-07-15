using AutoMapper;
using SkudTransfer.Transfers;
using SkudTransferApi.Contexts;
using Old = ASPWeb.Models;
using New = ControllerDomain.Entities;
using Microsoft.EntityFrameworkCore;

namespace SkudTransferApi.Transfers
{
    public class EventTransfer : Transfer
    {
        private readonly IMapper _mapper;
        private readonly OldSkudContext _oldContext;
        private readonly NewSkudContext _newContext;
        public EventTransfer(IMapper mapper, OldSkudContext oldContext, NewSkudContext newContext) 
        {
            _mapper = mapper;
            _oldContext = oldContext;
            _newContext = newContext;
        }
        public async override Task DoTransfer()
        {
            var olds = await _oldContext.Set<Old.Events>().AsNoTracking().Take(1000).ToListAsync();
            foreach (var old in olds)
            {
                var controller = await _newContext
                    .Set<New.Controller>()
                    .AsNoTracking()
                    .Include(x => x.ControllerLocation)
                    .FirstOrDefaultAsync(x => x.Sn == old.sn.ToString());
                //if (controller == null || controller.ControllerLocation == null) { continue; }
                if (controller == null) { continue; }
                New.Worker? workerNew = null;
                var workerOld = await _oldContext.Set<Old.Workers>().AsNoTracking().FirstOrDefaultAsync(x => x.Id == old.workerId);
                if (workerOld != null)
                {
                    workerOld.LastName ??= string.Empty;
                    workerOld.FirstName ??= string.Empty;
                    workerOld.FatherName ??= string.Empty;
                    workerNew = await _newContext
                        .Set<New.Worker>()
                        .AsNoTracking()
                        .FirstOrDefaultAsync(x => x.LastName.Trim().ToLower() + x.FirstName.Trim().ToLower() + x.FatherName.Trim().ToLower()
                            == workerOld.LastName.Trim().ToLower() + workerOld.FirstName.Trim().ToLower() + workerOld.FatherName.Trim().ToLower());
                }
                var newEvent = _mapper.Map<New.Event>(old);
                newEvent.Worker = workerNew;
                newEvent.ControllerLocation = controller.ControllerLocation;
                var entity = await _newContext.Set<New.Event>().FirstOrDefaultAsync(x => x.EventTypeId == newEvent.EventTypeId && x.Create == newEvent.Create);
                _newContext.Add(newEvent);
            }
        }
    }
}
