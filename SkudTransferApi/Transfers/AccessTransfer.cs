using AutoMapper;
using SkudTransfer.Transfers;
using SkudTransferApi.Contexts;
using Old = ASPWeb.Models;
using New = ControllerDomain.Entities;
using Microsoft.EntityFrameworkCore;
using ControllerDomain.Entities;

namespace SkudTransferApi.Transfers
{
    public class AccessTransfer : Transfer
    {
        private readonly IMapper _mapper;
        private readonly OldSkudContext _oldContext;
        private readonly NewSkudContext _newContext;
        public AccessTransfer(IMapper mapper, OldSkudContext oldContext, NewSkudContext newContext)
        {
            _mapper = mapper;
            _oldContext = oldContext;
            _newContext = newContext;
        }
        public async override Task DoTransfer()
        {
            var controllersWithLocations = await _newContext.Set<New.Controller>().Include(x => x.ControllerLocation).AsNoTracking().ToListAsync();
            var workers = await _newContext.Set<New.Worker>().Include(x => x.Accesses).ThenInclude(x => x.ControllerLocation).ThenInclude(x => x!.Controller).ToListAsync();
            foreach(var worker in workers)
            {
                var oldWorker = await _oldContext.Set<Old.Workers>().AsNoTracking().FirstOrDefaultAsync(x => x.LastName == worker.LastName && x.FirstName == worker.FirstName && x.FatherName == worker.FatherName);
                if (oldWorker == null)
                {
                    continue;
                }
                var relationsControllersWorkers = await _oldContext.Set<Old.RelationsControllersWorkers>().AsNoTracking().Where(x => x.workerId == oldWorker.Id).ToListAsync();
                var sns = relationsControllersWorkers.Select(x => x.sn.ToString()).ToList();
                foreach (var rel in relationsControllersWorkers)
                {
                    var access = worker.Accesses
                        .FirstOrDefault(x =>
                            x.ControllerLocation != null
                            && x.ControllerLocation.Controller != null
                            && rel.sn.ToString().Equals(x.ControllerLocation.Controller.Sn));
                    if (access != null)
                    {
                        access.DateBlock = worker.DateBlock;
                        if(rel.reader == 1)
                            access.Enterance = true;
                        if (rel.reader == 2)
                            access.Exit = true;
                    }
                    else
                    {
                        var controller = controllersWithLocations.FirstOrDefault(x => x.Sn == rel.sn.ToString());
                        if (controller == null || controller.ControllerLocation == null)
                        {
                            continue;
                        }
                        access = new New.Access() { Worker = worker, DateBlock = worker.DateBlock, ControllerLocation = controller.ControllerLocation };
                        if (rel.reader == 1)
                            access.Enterance = true;
                        if (rel.reader == 2)
                            access.Exit = true;
                        await _newContext.AddAsync(access);
                    }
                }
            }
            await _newContext.SaveChangesAsync();
        }
    }
}
