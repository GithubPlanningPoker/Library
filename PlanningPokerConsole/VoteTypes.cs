using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanningPokerConsole
{
    public enum VoteTypes
    {//0, half, 1, 2, 3, 5, 8, 13, 20, 40, 100, inf, ?, break
        Zero,
        Half,
        One,
        Two,
        Three,
        Five,
        Eight,
        Thirteen,
        Twenty,
        Fourty,
        OneHundred,
        Infinite,
        QuestionMark,
        Break
    }

    public static class VoteTypesExtension
    {
        public static string ToAPIString(this VoteTypes voteType)
        {
            switch (voteType)
            {
                case VoteTypes.Zero: return "0";
                case VoteTypes.Half: return "half";
                case VoteTypes.One: return "1";
                case VoteTypes.Two: return "2";
                case VoteTypes.Three: return "3";
                case VoteTypes.Five: return "5";
                case VoteTypes.Eight: return "8";
                case VoteTypes.Thirteen: return "13";
                case VoteTypes.Twenty: return "20";
                case VoteTypes.Fourty: return "40";
                case VoteTypes.OneHundred: return "100";
                case VoteTypes.Infinite: return "inf";
                case VoteTypes.QuestionMark: return "?";
                case VoteTypes.Break: return "break";
                default:
                    throw new ArgumentException("Unknown vote type.");
            }
        }

        public static VoteTypes Parse(string input)
        {
            switch (input)
            {
                case "0": return VoteTypes.Zero;
                case "half": return VoteTypes.Half;
                case "1": return VoteTypes.One;
                case "2": return VoteTypes.Two;
                case "3": return VoteTypes.Three;
                case "5": return VoteTypes.Five;
                case "8": return VoteTypes.Eight;
                case "13": return VoteTypes.Thirteen;
                case "20": return VoteTypes.Twenty;
                case "40": return VoteTypes.Fourty;
                case "100": return VoteTypes.OneHundred;
                case "inf": return VoteTypes.Infinite;
                case "?": return VoteTypes.QuestionMark;
                case "break": return VoteTypes.Break;
                default:
                    throw new ArgumentException("Unknown vote type.");
            }
        }
    }
}
