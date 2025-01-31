using AutoMapper;
using SkudTransfer.Transfers;
using SkudTransferApi.Contexts;
using Old = ASPWeb.Models;
using New = ControllerDomain.Entities;
using Microsoft.EntityFrameworkCore;
using ControllerDomain.Entities;
using ASPWeb.Models;

namespace SkudTransferApi.Transfers
{
    public class AccessGroupTransfer : Transfer
    {
        private readonly IMapper _mapper;
        private readonly OldSkudContext _oldContext;
        private readonly NewSkudContext _newContext;
        public AccessGroupTransfer(IMapper mapper, OldSkudContext oldContext, NewSkudContext newContext)
        {
            _mapper = mapper;
            _oldContext = oldContext;
            _newContext = newContext;
        }
        public async override Task DoTransfer()
        {
            var oldAccessGroup = await _oldContext.Set<Old.AccessGroups>().AsNoTracking().ToListAsync();
            foreach (var accessGroup in oldAccessGroup)
            {
                var inNewAccessGroup = await _newContext.Set<New.AccessGroup>().AsNoTracking().FirstOrDefaultAsync(x => x.Name == accessGroup.accessGroup);
                if (inNewAccessGroup == null)
                {
                    var newAccessGroup = new AccessGroup() { Name = accessGroup.accessGroup };
                    await _newContext.AddAsync(newAccessGroup);
                }
            }
            await _newContext.SaveChangesAsync();
            var oldAccessGroupLocation = await _oldContext.Set<Old.RelationsControllersAccessGroups>().AsNoTracking().ToListAsync();
            var newControllerLocation = await _newContext.Set<New.Controller>().AsNoTracking().Include(x => x.ControllerLocation).ToListAsync();
            //var newaccessGroupAccess = await _newContext.Set<New.AccessGroupAccess>().AsNoTracking().ToListAsync();
            var allAccesGroup = await _newContext.Set<New.AccessGroup>().AsNoTracking().ToListAsync();
            var relationsToAdd = new List<AccessGroupAccess>();
            foreach (var controller in oldAccessGroupLocation)
            {
                var newController = newControllerLocation.FirstOrDefault(x => x.Sn == controller.sn.ToString());
                var newGroup = allAccesGroup.FirstOrDefault(x => x.Name == controller.accessGroup);
                if (newGroup != null && newController != null && newController.ControllerLocationId != null)
                {
                    var findRelation = await _newContext.Set<New.AccessGroupAccess>().AsNoTracking().FirstOrDefaultAsync(x => x.AccessGroupId == newGroup.Id && x.ControllerLocationId == newController.ControllerLocationId);
                    var findLocal = relationsToAdd.FirstOrDefault(x => x.AccessGroupId == newGroup.Id && x.ControllerLocationId == newController.ControllerLocationId);

                    if (findRelation != null || findLocal != null)
                    {
                        if (findLocal == null)
                        {
                            if (controller.reader == 1)
                            {
                                findRelation.Enterance = true;
                            }
                            else
                            {
                                findRelation.Exit = true;
                            }
                        
                            _newContext.Update(findRelation);
                        }
                        else
                        {
                            if (controller.reader == 1)
                            {
                                findLocal.Enterance = true;
                            }
                            else
                            {
                                findLocal.Exit = true;
                            }
                        }
                    }
                    else
                    {
                        var newRelation = new AccessGroupAccess() { AccessGroupId = newGroup.Id, ControllerLocationId = (int)newController.ControllerLocationId };
                        if (controller.reader == 1)
                        {
                            newRelation.Enterance = true;
                        }
                        else
                        {
                            newRelation.Exit = true;
                        }
                        relationsToAdd.Add(newRelation);
                    }

                }

            }
            await _newContext.AddRangeAsync(relationsToAdd);
            await _newContext.SaveChangesAsync();
        }
    }
}
