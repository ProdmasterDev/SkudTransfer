using AutoMapper;
using SkudTransfer.Transfers;
using SkudTransferApi.Contexts;
using Old = ASPWeb.Models;
using New = ControllerDomain.Entities;
using Microsoft.EntityFrameworkCore;

namespace SkudTransferApi.Transfers
{
    public class WorkerGroupTransfer : Transfer
    {
        private readonly IMapper _mapper;
        private readonly OldSkudContext _oldContext;
        private readonly NewSkudContext _newContext;
        public WorkerGroupTransfer(IMapper mapper, OldSkudContext oldContext, NewSkudContext newContext)
        {
            _mapper = mapper;
            _oldContext = oldContext;
            _newContext = newContext;
        }
        public async override Task DoTransfer()
        {
            var workerGroupsOld = await _oldContext.Set<Old.Groups>().AsNoTracking().ToListAsync();
            var workerGroupsToUpdate = _mapper.Map<IEnumerable<New.WorkerGroup>>(workerGroupsOld);
            foreach (var workerGroup in workerGroupsToUpdate)
            {
                var entity = await _newContext.Set<New.WorkerGroup>().FirstOrDefaultAsync(x => x.Name == workerGroup.Name);
                if (entity == null)
                {
                    entity = workerGroup;
                    _newContext.Add(entity);
                }
                else
                {
                    entity = _mapper.Map(workerGroup, entity);
                    _newContext.Update(entity);
                }
            }
            await _newContext.SaveChangesAsync();
        }
    }
}
