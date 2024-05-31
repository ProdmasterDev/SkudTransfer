using AutoMapper;
using SkudTransferApi.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Old = ASPWeb.Models;
using New = ControllerDomain.Entities;
using Microsoft.EntityFrameworkCore;

namespace SkudTransfer.Transfers
{
    public class ControllerTransfer : Transfer
    {
        private readonly IMapper _mapper;
        private readonly OldSkudContext _oldContext;
        private readonly NewSkudContext _newContext;
        public ControllerTransfer(IMapper mapper, OldSkudContext oldContext, NewSkudContext newContext) 
        { 
            _mapper = mapper; 
            _oldContext = oldContext; 
            _newContext = newContext; 
        }
        public async override Task DoTransfer()
        {
            var controllersOld = await _oldContext.Set<Old.Controllers>().AsNoTracking().ToListAsync();
            var controllersToUpdate = _mapper.Map<IEnumerable<New.Controller>>(controllersOld);
            foreach (var controller in controllersToUpdate)
            {
                var entity = await _newContext.Set<New.Controller>().FirstOrDefaultAsync(x => x.Sn == controller.Sn);
                if(entity == null)
                {
                    entity = controller;
                    _newContext.Add(entity);
                }
                else
                {
                    entity = _mapper.Map(controller, entity);
                    _newContext.Update(entity);
                }
            }
            await _newContext.SaveChangesAsync();
        }
    }
}
