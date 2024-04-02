// using System;
// using System.Collections.Generic;

// public class PorkCard
// {
//    public enum Suit
//    {
//        Club,
//        Spade,
//        Heart,
//        Diamond
//    }

//    public interface ICard
//    {
//        Suit GetSuit();
//        int GetNumber();
//    }

//    public class Card : ICard
//    {
//        private Suit Suit;
//        private int Number;
//        public Card(Suit suit, int number)
//        {
//            this.Suit = suit;
//            this.Number = number;
//        }
//        public Suit GetSuit()
//        {
//            return this.Suit;
//        }

//        public int GetNumber()
//        {
//            return this.Number;
//        }
//    }

//    public class CardDeck
//    {
//        public List<Card> Cards;
//        public CardDeck()
//        {
//            Cards = new List<Card>();
//            var suits = Enum.GetValues(typeof(Suit));
//            foreach (Suit suit in suits)
//            {
//                for (int i = 1; i <= 13; i++)
//                    Cards.Add(new Card(suit, i));
//            }
//        }

//        public void ShuffleCard()
//        {
//            Random random = new Random();
//            for(int i = 0; i < 52; i++)
//            {
//                int j = random.Next(0, 51);
//                Card temp = Cards[i];
//                Cards[i] = Cards[j];
//                Cards[j] = temp;
//            }
//        }

//        public Card DrawCard()
//        {
//            Card firstCard = Cards[0];
//            Cards.RemoveAt(0);
//            return firstCard;
//        }

//        public Card DrawCardRandomly()
//        {
//            Random random = new Random();
//            int i = random.Next(0, Cards.Count);
//            Card card = Cards[i];
//            Cards.RemoveAt(i);
//            return card;
//        }
//    }

//    static void Main(string[] args)
//    {
//        CardDeck cardDeck = new CardDeck();
//        // shuffle card deck
//        cardDeck.ShuffleCard();

//        // draw a card randomly
//        Card card1 = cardDeck.DrawCardRandomly();
//    }
// }