using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Word = Microsoft.Office.Interop.Word;

namespace FridayLib.Word_Module
{
    class Common
    {
        internal static Word.Application GetApplication()
        {
            return new Word.Application();
        }
        
        internal static Word.Document OpenDoc(Word.Application wordApp,string address)
        {
            try
            {
                //Подготавливаем данные для открытия
                Object filename = address;
                Object confirmConversions = true;
                Object readOnly = false;
                Object addToRecentFiles = true;
                Object passwordDocument = Type.Missing;
                Object passwordTemplate = Type.Missing;
                Object revert = false;
                Object writePasswordDocument = Type.Missing;
                Object writePasswordTemplate = Type.Missing;
                Object format = Type.Missing;
                Object encoding = Type.Missing;
                Object oVisible = Type.Missing;
                Object openConflictDocument = Type.Missing;
                Object openAndRepair = Type.Missing;
                Object documentDirection = Type.Missing;
                Object noEncodingDialog = false;
                Object xmlTransform = Type.Missing;
                return wordApp.Documents.Open(ref filename,
                            ref confirmConversions, ref readOnly, ref addToRecentFiles,
                            ref passwordDocument, ref passwordTemplate, ref revert,
                            ref writePasswordDocument, ref writePasswordTemplate,
                            ref format, ref encoding, ref oVisible,
                            ref openAndRepair, ref documentDirection, ref noEncodingDialog, ref xmlTransform);
            }
            catch (Exception ex)
            {
                Service.OnErrorInLibrary(string.Format("Ошибка создания документа: {0}",ex.Message));
                return null;
            }
        }

        internal static void SaveDocument(Word.Document document, string saveTo)
        {
            try
            {
                //Подготавливаем параметры для сохранения документа
                Object fileName = saveTo;
                Object fileFormat = Word.WdSaveFormat.wdFormatDocumentDefault;
                Object lockComments = false;
                Object password = "";
                Object addToRecentFiles_out = false;
                Object writePassword = "";
                Object readOnlyRecommended = false;
                Object embedTrueTypeFonts = false;
                Object saveNativePictureFormat = false;
                Object saveFormsData = false;
                Object saveAsAOCELetter = Type.Missing;
                Object encoding_out = Type.Missing;
                Object insertLineBreaks = Type.Missing;
                Object allowSubstitutions = Type.Missing;
                Object lineEnding = Type.Missing;
                Object addBiDiMarks = Type.Missing;
                document.SaveAs(ref fileName,
                        ref fileFormat, ref lockComments,
                        ref password, ref addToRecentFiles_out, ref writePassword,
                        ref readOnlyRecommended, ref embedTrueTypeFonts,
                        ref saveNativePictureFormat, ref saveFormsData,
                        ref saveAsAOCELetter, ref encoding_out, ref insertLineBreaks,
                        ref allowSubstitutions, ref lineEnding, ref addBiDiMarks);
            }
            catch (Exception ex)
            {
                Service.OnErrorInLibrary(string.Format("Ошибка сохранения документа: {0}", ex.Message));
            }
        }

        internal static void CloseApp(Word.Application wordapp)
        {
            //сохранять изменения
            Object saveChanges = Word.WdSaveOptions.wdPromptToSaveChanges;
            //формат хранения документа
            Object originalFormat = Word.WdOriginalFormat.wdWordDocument;
            //Какая-то ерунда с пользователями
            Object routeDocument = Type.Missing;
            wordapp.Quit(ref saveChanges,
                         ref originalFormat, ref routeDocument);
        }
    }
}
