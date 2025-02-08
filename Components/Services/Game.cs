using System;
using System.Collections.Immutable;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Match.Models;

namespace Match.Services
{
    public interface IGameService
    {
        Engine NewGame();
        Engine UnFlip(Engine state);
        Engine FlipCard(Engine state, Card card);
    }

    public class GameService : IGameService
    {
        private readonly Random _random = new();
        private readonly List<string> _cardNumbers = new() { "one", "two", "three", "four" };

        public Engine NewGame()
        {
            var cards = _cardNumbers.Distinct()
                .SelectMany((name, index) => Enumerable.Range(1, 2)
                    .Select(i => new Card
                    {
                        Id = (index * 2) + i,
                        Name = name,
                        ImageUrl = $"/images/cards/{name}.png"
                    }))
                .ToList();

            var newCards = cards.OrderBy(x => _random.Next()).ToList();
            return new Engine
            {
                Winner = false,
                Animating = false,
                Cards = newCards.ToImmutableList()
            };
        }

        public Engine UnFlip(Engine state)
        {
          var newCards = state.Cards.Select(card => card with { Flipped = false }).ToList();

          return new Engine
          {
            Winner = state.Winner,
            Animating = false,
            Cards = newCards.ToImmutableList()
          };
        }

        public Engine FlipCard(Engine state, Card card)
        {
          if (state.Animating) return state;

          var stateOne = Flip(state, card);
          var stateTwo = AttemptMatch(stateOne);
          var stateThree = CheckWinner(stateTwo);

          return stateThree;
        }

        private Engine Flip(Engine state, Card card)
        {
            var newCards = state.Cards.Select(item =>
                card.Id == item.Id? item with { Flipped = true }: item
            ).ToList();

          return new Engine
          {
            Winner = state.Winner,
            Animating = state.Animating,
            Cards = newCards.ToImmutableList()
          };
        }

        private Engine AttemptMatch(Engine state)
        {
            var flippedCards = state.Cards.Where(card => card.Flipped).ToList();

            if (flippedCards.Count != 2) return state;

            var one = flippedCards[0];
            var two = flippedCards[1];

            if (one.Name == two.Name)
            {
              var newCards = state.Cards.Select(item =>
                  item.Id == one.Id? one with { Flipped = false, Paired = true }:
                  item.Id == two.Id? two with { Flipped = false, Paired = true }:
                  item
              ).ToList();

              return new Engine
              {
                Winner = state.Winner,
                Animating = state.Animating,
                Cards = newCards.ToImmutableList()
              };
            } else {
              return new Engine
              {
                Animating = true,
                Winner = state.Winner,
                Cards = state.Cards
              };
            }
        }

        private Engine CheckWinner(Engine state)
        {
            var pairedCards = state.Cards.Where(card => card.Paired).ToList();
            var won = pairedCards.Count == 8;
            return new Engine
            {
              Animating = state.Animating,
              Winner = won,
              Cards = state.Cards
            };
        }
    }
}
