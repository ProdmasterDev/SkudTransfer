using AutoMapper;
using SkudTransfer.Transfers;
using SkudTransferApi.Contexts;
using Old = ASPWeb.Models;
using New = ControllerDomain.Entities;
using Microsoft.EntityFrameworkCore;

namespace SkudTransferApi.Transfers
{
    public class CardTransfer : Transfer
    {
        private readonly IMapper _mapper;
        private readonly OldSkudContext _oldContext;
        private readonly NewSkudContext _newContext;
        public CardTransfer(IMapper mapper, OldSkudContext oldContext, NewSkudContext newContext)
        {
            _mapper = mapper;
            _oldContext = oldContext;
            _newContext = newContext;
        }
        public async override Task DoTransfer()
        {
            var cardsOld = await _oldContext.Set<Old.Cards>().AsNoTracking().ToListAsync();
            var cardsToUpdate = _mapper.Map<IEnumerable<New.Card>>(cardsOld);
            foreach (var card in cardsToUpdate)
            {
                var entity = await _newContext.Set<New.Card>().FirstOrDefaultAsync(x => x.CardNumb16 == card.CardNumb16);
                if (entity == null)
                {
                    entity = card;

                    var workerId = card.WorkerId;
                    entity.WorkerId = null;
                    var oldWorker = await _oldContext.Set<Old.Workers>().AsNoTracking().FirstOrDefaultAsync(x => x.Id == workerId);
                    var worker = _mapper.Map<New.Worker>(oldWorker);
                    if (worker != null)
                    {
                        entity.Worker = await _newContext
                            .Set<New.Worker>()
                            .FirstOrDefaultAsync(x => x.LastName == worker.LastName && x.FirstName == worker.FirstName && x.FatherName == worker.FatherName);
                        if (entity.Worker != null)
                        {
                            entity.WorkerId = entity.Worker.Id;
                        }
                    }

                    _newContext.Add(entity);
                }
                else
                {
                    entity = _mapper.Map(card, entity);

                    var workerId = card.WorkerId;
                    entity.WorkerId = null;
                    var oldWorker = await _oldContext.Set<Old.Workers>().AsNoTracking().FirstOrDefaultAsync(x => x.Id == workerId);
                    var worker = _mapper.Map<New.Worker>(oldWorker);
                    if (worker != null)
                    {
                        entity.Worker = await _newContext
                            .Set<New.Worker>()
                            .FirstOrDefaultAsync(x => x.LastName == worker.LastName && x.FirstName == worker.FirstName && x.FatherName == worker.FatherName);
                        if (entity.Worker != null)
                        {
                            entity.WorkerId = entity.Worker.Id;
                        }
                    }

                    _newContext.Update(entity);
                }
            }
            await _newContext.SaveChangesAsync();
        }
    }
}
