using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Word = Microsoft.Office.Interop.Word;

namespace FridayLib.Word_Module
{
    public class DocumentCreation
    {
        public static void CreateRequest(ControlledApp app)
        {
            try
            {
                var wordApp = Common.GetApplication();
                var document = Common.OpenDoc(wordApp, Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"Res","Sample request.docx"));
                document.Paragraphs[1].Range.Text = string.Format("Дата: {0}\r\n", DateTime.Now.ToString("dd.MM.yyyy"));
                //Меняем название программы в шапке
                var rest = 60 - app.Name.Length;
                var appNameString = "";
                for (int i = 0; i < (int)(rest / 2); i++)
                {
                    appNameString += "_";
                }
                appNameString += app.Name;
                for (int i = 0; i < (int)(rest / 2); i++)
                {
                    appNameString += "_";
                }
                document.Paragraphs[10].Range.Text = string.Format("{0} в составе\r\n", appNameString);
                document.Paragraphs[10].Range.Font.Bold = 0;
                document.Paragraphs[10].Range.Font.Underline = Word.WdUnderline.wdUnderlineSingle;
                document.Paragraphs[10].Format.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                document.Paragraphs[10].Format.LeftIndent = 0;
                document.Paragraphs[10].Format.FirstLineIndent = 0;
                object unit;
                object count;
                object extend;
                //перемещаем курсор в начало документа
                unit = Word.WdUnits.wdStory;
                extend = Word.WdMovementType.wdMove;
                wordApp.Selection.HomeKey(ref unit, ref extend);
                //перемещаем курсор в начало 10 параграфа
                unit = Word.WdUnits.wdParagraph;
                count = 9;
                extend = Word.WdMovementType.wdMove;
                wordApp.Selection.MoveDown(ref unit, ref count, ref extend);
                //перемещаем курсор до слова "в составе"
                unit = Word.WdUnits.wdCharacter;
                count = 60;
                extend = Word.WdMovementType.wdMove;
                wordApp.Selection.MoveRight(ref unit, ref count, ref extend);
                //выделяем нужные слова
                unit = Word.WdUnits.wdWord;
                count = 2;
                extend = Word.WdMovementType.wdExtend;
                wordApp.Selection.MoveRight(ref unit, ref count, ref extend);
                //меняем стиль выделения
                wordApp.Selection.Font.Bold = 2;
                wordApp.Selection.Font.Underline = Word.WdUnderline.wdUnderlineNone;

                //Меняем название МПСА в шапке
                string saName = "Система автоматики";
                rest = 70 - saName.Length;
                var saString = "";
                for (int i = 0; i < (int)(rest / 2); i++)
                {
                    saString += "_";
                }
                saString += saName;
                for (int i = 0; i < (int)(rest / 2); i++)
                {
                    saString += "_";
                }
                document.Paragraphs[12].Range.Text = string.Format("{0}\r\n", saString);
                document.Paragraphs[12].Range.Font.Bold = 0;
                document.Paragraphs[12].Range.Font.Underline = Word.WdUnderline.wdUnderlineSingle;
                document.Paragraphs[12].Format.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                document.Paragraphs[12].Format.LeftIndent = 0;
                document.Paragraphs[12].Format.FirstLineIndent = 0;

                //Меняем Имя приложения в таблице
                document.Paragraphs[19].Range.Text = string.Format("{0}", app.Name);

                //Меняем версию ппо
                document.Paragraphs[23].Range.Text = string.Format("{0}", app.MainFileReleaseVersion);

                //меняем разработчика
                document.Paragraphs[27].Range.Text = string.Format("{0}", "АО \"НПО \"Спецэлектромеханика\"");

                //меняем контрольную сумму
                document.Paragraphs[31].Range.Text = string.Format("{0}", app.MainFileReleaseHash);

                //меняем дату выпуска ППО
                document.Paragraphs[35].Range.Text = string.Format("{0}", app.MainFileReleaseDate.Remove(app.MainFileReleaseDate.IndexOf(" ")));

                //меняем описание ППО
                document.Paragraphs[39].Range.Text = string.Format("{0}", app.Description);

                Common.SaveDocument(document, Path.Combine(app.DocumentDirectory,"Заявка.docx"));

                Common.CloseApp(wordApp);

            }
            catch(Exception ex)
            {
                MainClass.OnErrorInLibrary(string.Format("Ошибка создания заявки для приложения {0}: {1}", app.Name, ex.Message));
            }
        }

        public static void CreateFormular(ControlledApp app)
        {
            try
            {
                var wordApp = Common.GetApplication();
                var document = Common.OpenDoc(wordApp, Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Res", "Sample formular.docx"));

                #region Редактирование документа
                //Заголовок программы
                document.Paragraphs[1].Range.Text = string.Format("{0}\r\n", app.Name);
                document.Paragraphs[1].Range.Font.Bold = 2;
                document.Paragraphs[1].Format.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                //1.1 Тип ППО
                var parData = document.Paragraphs[4].Range.Text;
                parData = string.Format("{0}: {1}.\r\n", parData.Remove(parData.IndexOf(": ")), app.Parent.Task.ToString());
                document.Paragraphs[4].Range.Text = parData;
                document.Paragraphs[4].Range.Font.Bold = 1;
                //1.2 Категория ППО

                //1.3.2 Назначение каждого из звеньев

                //1.3.3 Исполняемые среды каждого из звеньев

                //1.4.2 Назначение каждого из приложений

                //2.1 Совместимые ОС с указанием версий

                //2.2 Совместимые SCADA с указанием версий

                //2.3 Совместимые СЗИ с указанием версий

                //2.4 Другие необходимые типы ПО

                //3.1 Тип идентификации и аутентификации

                //3.2 Тип авторизации

                //3.3 Предполагаемые категории пользователей

                //4.1 Состав разделяемых и локально хранимых данных

                //4.2 Используемая СУБД

                //4.4 Используемые механизмы с средства хранения локальных данных

                //5.2 Компоненты и платформы, используемые для функционирования

                //5.3 Компоненты и платформы, используемые для сборки

                //5.4 Средства предоставления отчетности

                //6.2 Тип установщика

                #endregion

                Common.SaveDocument(document, Path.Combine(app.DocumentDirectory, "Описание принятых решений.docx"));
                Common.CloseApp(wordApp);
            }
            catch (Exception ex)
            {
                MainClass.OnErrorInLibrary(string.Format("Ошибка создания описания принятых решений для приложения {0}: {1}", app.Name, ex.Message));
            }
        }
    }
}
