using AutoMapper;
using SkudTransfer.Transfers;
using SkudTransferApi.Contexts;
using Old = ASPWeb.Models;
using New = ControllerDomain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System;

namespace SkudTransferApi.Transfers
{
    public class WorkerTransfer : Transfer
    {
        private readonly IMapper _mapper;
        private readonly OldSkudContext _oldContext;
        private readonly NewSkudContext _newContext;
        public WorkerTransfer(IMapper mapper, OldSkudContext oldContext, NewSkudContext newContext)
        {
            _mapper = mapper;
            _oldContext = oldContext;
            _newContext = newContext;
        }
        public async override Task DoTransfer()
        {
            var workersOld = await _oldContext.Set<Old.Workers>().AsNoTracking().ToListAsync();
            workersOld.ForEach(x => { x.LastName ??= string.Empty; x.FirstName ??= string.Empty; x.FatherName ??= string.Empty; });
            var workersToUpdate = _mapper.Map<IEnumerable<New.Worker>>(workersOld);
            foreach (var worker in workersToUpdate)
            {
                var entity = await _newContext.Set<New.Worker>().FirstOrDefaultAsync(x => x.LastName == worker.LastName && x.FirstName == worker.FirstName && x.FatherName == worker.FatherName);
                if (entity == null)
                {
                    entity = worker;
                    var dateBlock = new DateTime();
                    var old = workersOld.First(x => x.LastName == worker.LastName && x.FirstName == worker.FirstName && x.FatherName == worker.FatherName);
                    if (DateTime.TryParseExact(old.lockDate, "yyyy-MM-ddTHH:mm", null, System.Globalization.DateTimeStyles.None, out dateBlock))
                    {
                        entity.DateBlock = dateBlock.ToUniversalTime();
                    }
                    if(old.group!= null && old.group.Count() > 0)
                    {
                        entity.Group = await _newContext.Set<New.WorkerGroup>().FirstOrDefaultAsync(x => x.Name == old.group);
                    }
                    _newContext.Add(entity);
                }
                else
                {
                    entity = _mapper.Map(worker, entity);
                    var dateBlock = new DateTime();
                    var old = workersOld.First(x => x.LastName == worker.LastName && x.FirstName == worker.FirstName && x.FatherName == worker.FatherName);
                    if (DateTime.TryParseExact(old.lockDate, "yyyy-MM-ddTHH:mm", null, System.Globalization.DateTimeStyles.None, out dateBlock))
                    {
                        entity.DateBlock = dateBlock.ToUniversalTime();
                    }
                    if (old.group != null && old.group.Count() > 0)
                    {
                        entity.Group = await _newContext.Set<New.WorkerGroup>().FirstOrDefaultAsync(x => x.Name == old.group);
                    }
                    _newContext.Update(entity);
                }
            }
            await _newContext.SaveChangesAsync();
        }
    }
}
