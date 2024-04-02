// using System.Text;

// namespace StringLibraryTest
// {
//     public class Team
//     {
//         public char Name { get; set; }
//         public int[] Rank { get; set; }

//         public Team(char name, int teamSize)
//         {
//             this.Name = name;
//             this.Rank = new int[teamSize];
//         }
//     }
//     [TestClass]
//     public class voting
//     {


//         [TestMethod]
//         public void RankTeams()
//         {

//             var votes = new string[] { "ABC", "ACB", "ABC", "ACB", "ACB" };
//             var teamSize = votes[0].Length;
//             var teams = new Team[26];
//             var rankedTeams = new StringBuilder();

//             for (int i = 0; i < 26; ++i)
//                 teams[i] = new Team((char)('A' + i), teamSize);

//             foreach (var vote in votes)
//                 for (int i = 0; i < teamSize; i++)
//                     teams[vote[i] - 'A'].Rank[i]++;

//             Array.Sort(teams, (a, b) =>
//             {
//                 for (var i = 0; i < a.Rank.Length; ++i)
//                     if (a.Rank[i] > b.Rank[i])
//                         return -1;
//                     else if (a.Rank[i] < b.Rank[i])
//                         return 1;

//                 return a.Name - b.Name;
//             });


//             for (int i = 0; i < teamSize; ++i)
//                 rankedTeams.Append(teams[i].Name);

//             var t = rankedTeams.ToString();
//         }


//         [TestMethod]
//         public void SortingArry()
//         {

//             var votes = new string[] { "ABC", "ACB", "ABC", "ACB", "ACB" };

//             var numOfVotePositions = votes[0].Length;
//             int[][] teamStats = new int[26][];
//             var team = new int[26];

//             for (int i = 0; i < 26; i++)
//             {
//                 teamStats[i] = new int[numOfVotePositions];
//                 team[i] = i;
//             }

//             foreach (var vote in votes)
//             {
//                 for (int i = 0; i < numOfVotePositions; i++)
//                 {
//                     var teamIdx = vote[i] - 'A';
//                     teamStats[teamIdx][i]++;
//                 }
//             }
//             Array.Sort(team, (a, b) =>
//             {
//                 return a - b;
//             });


//             //Sort team array with the help of teamStats array
//             Array.Sort(team, (a, b) =>
//             {
//                 for (int i = 0; i < numOfVotePositions; i++)
//                 {
//                     if (teamStats[a][i] != teamStats[b][i])
//                     {
//                         return teamStats[b][i] - teamStats[a][i]; // vote order, if vote 0 same, then test vote 1 till all vote, if still tie, then go to a-b;
//                     }
//                 }
//                 return a - b;//alphabatic order

//             });
//             var sb = new StringBuilder();

//             for (int i = 0; i < numOfVotePositions; i++)
//             {
//                 sb.Append((char)(team[i] + 'A'));
//             }

//             var result = sb.ToString();

//         }


//         [TestMethod]
//         public void Sorting()
//         {
//             var votes = new string[] { "ABC", "ACB", "ABC", "ACB", "ACB" };

//             int n = votes.Length;
//             int voters = votes[0].Length;

//             // position counte
//             Dictionary<char, int>[] arr = new Dictionary<char, int>[voters];
//             for (int j = 0; j < voters; j++)
//             {
//                 // frequency for the jth place
//                 arr[j] = new Dictionary<char, int>();
//                 for (int i = 0; i < n; i++)
//                 {
//                     char c = votes[i][j];
//                     arr[j][c] = 1 + (arr[j].ContainsKey(c) ? arr[j][c] : 0);
//                 }
//             }

//             // create a character array of all the teams
//             char[] ans = votes[0].ToCharArray();

//             // sort the teams using the position counter
//             Array.Sort(ans, (c1, c2) =>
//             {
//                 for (int i = 0; i < voters; i++)
//                 {
//                     int ch1 = arr[i].ContainsKey(c1) ? arr[i][c1] : 0;
//                     int ch2 = arr[i].ContainsKey(c2) ? arr[i][c2] : 0;
//                     if (ch1 == ch2) continue;
//                     return ch2 - ch1;
//                 }
//                 // if the execution reached this point, it mean the teams are tied. So, we compare them alphabetically.
//                 return c1 - c2;
//             });

//             var r = string.Join("", ans);
//         }

//     }
// }
