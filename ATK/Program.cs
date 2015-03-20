using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ATK.SpellerService;

namespace ATK
{
     class Program
     {
          static void Main(string[] args)
          {
               using (SpellerServiceClient SpellerClient = new SpellerServiceClient("HTTPS_ISpellerService"))
               {
                    string AppID = "XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX";
                    ErroneousWord[] WrongWords = null;
                    string CorrectedText = null;
                    string inputText = "تجب تقديم دراسه الجدوي ومعاهم تشرير مقصل ";
                    Dictionary<string, List<string>> Suggestions = null;

                    Console.WriteLine("Calling API .....");

                    // Detect mistakes, get suggestions for wrong words and autocorrect the input text
                    SpellerErrorCode RetCode = SpellerClient.AutocorrectAndSuggest(AppID, inputText, false, out CorrectedText, out WrongWords);

                    if (RetCode == SpellerErrorCode.Success)
                    {
                         // Display the suggested autocorrection for the whole text
                        string  textAutocorrectedLine = CorrectedText;

                        //Console.WriteLine(textAutocorrectedLine);
                        System.IO.File.WriteAllText(@"output.txt", textAutocorrectedLine);
                        Console.WriteLine("API Called Successfully, result written to the output file"); 
                         
                         // Prepare list of suggestions for non-autocorrected words
                         Suggestions = new Dictionary<string, List<string>>();

                         foreach (ErroneousWord WrongWord in WrongWords)
                         {
                              // Highlight the wrong words
                              Console.WriteLine("Wrong word position: " + WrongWord.Position + " and it length: " + WrongWord.Word.Length);

                              // Fill word suggestions for non-autocorrected words
                              Suggestions.Add(WrongWord.Word, new List<string>());
                              foreach (Correction SuggestedCorrection in WrongWord.Corrections)
                              {
                                   if (SuggestedCorrection.CorrectionType != CorrectionType.CORRECTION_TYPE_AUTO)
                                   {
                                        Suggestions[WrongWord.Word].Add(SuggestedCorrection.CorrectionText);
                                   }
                              }

                              System.IO.File.WriteAllLines(@"suggestions-"+WrongWord.Word+".txt", Suggestions[WrongWord.Word]);

                              // Some processing or user input ...

                         } 
                    }
                    else
                    {
                         Console.WriteLine("Failed with error code " + RetCode.ToString());
                    }
               }
          }
     }
}
