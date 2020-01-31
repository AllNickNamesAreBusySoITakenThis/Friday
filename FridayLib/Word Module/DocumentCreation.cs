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
        /// <summary>
        /// Переместить курсор на верх документа
        /// </summary>
        /// <param name="application">Приложение</param>
        static void MoveToStart(Word.Application application)
        {
            object unit;
            object extend;
            unit = Word.WdUnits.wdStory;
            extend = Word.WdMovementType.wdMove;
            application.Selection.HomeKey(ref unit, ref extend);
        }
        /// <summary>
        /// Переместить курсор с возможностью выделения
        /// </summary>
        /// <param name="application">Приложение</param>
        /// <param name="_units">Тип элементов смещения</param>
        /// <param name="_count">Количество элементов смещения</param>
        /// <param name="direction">Направление смещения</param>
        /// <param name="select">Требуется ли выделять</param>
        static void MoveSelect(Word.Application application, Word.WdUnits _units,int _count, MoveDirection direction, bool select)
        {
            object unit = _units; 
            object count = _count;
            object extend = select?Word.WdMovementType.wdExtend:Word.WdMovementType.wdMove;
            switch(direction)
            {
                case MoveDirection.Down:
                    application.Selection.MoveDown(ref unit, ref count, ref extend);
                    break;
                case MoveDirection.Up:
                    application.Selection.MoveUp(ref unit, ref count, ref extend);
                    break;
                case MoveDirection.Left:
                    application.Selection.MoveLeft(ref unit, ref count, ref extend);
                    break;
                case MoveDirection.Right:
                    application.Selection.MoveRight(ref unit, ref count, ref extend);
                    break;
            }
        }       
        /// <summary>
        /// Записать строку в описании принятых решений
        /// </summary>
        /// <param name="application">Приложение</param>
        /// <param name="document">Документ</param>
        /// <param name="paragraph">Номер строки</param>
        /// <param name="data">Ланные для записи</param>
        static void WriteLineInFormular(Word.Application application, Word.Document document,int paragraph, string data)
        {
            try
            {
                var parData = document.Paragraphs[paragraph].Range.Text;
                parData = string.Format("{0}: {1}.\r\n", parData.Remove(parData.IndexOf(": ")), data);
                document.Paragraphs[paragraph].Range.Text = parData;
                document.Paragraphs[paragraph].Range.Font.Bold = 0;
                MoveToStart(application);
                MoveSelect(application, Word.WdUnits.wdParagraph, paragraph - 1, MoveDirection.Down, false);
                MoveSelect(application, Word.WdUnits.wdCharacter, parData.IndexOf(": ") + 1, MoveDirection.Right, true);
                application.Selection.Font.Bold = 2;
            }
            catch (Exception ex)
            {
                MainClass.OnErrorInLibrary(string.Format("Ошибка в формировании строки {0} описания принятых решений: {1}", paragraph, ex.Message));
            }
        }
        /// <summary>
        /// Создать заявку на анализ защищенности приложения
        /// </summary>
        /// <param name="app">Приложение</param>
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
        /// <summary>
        /// Создать описание принятых решений приложения
        /// </summary>
        /// <param name="app">Приложение</param>
        public static void CreateFormular(ControlledApp app)
        {
            try
            {
                Word.Application wordApp = Common.GetApplication();
                var document = Common.OpenDoc(wordApp, Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Res", "Sample formular.docx"));
                
                #region Редактирование документа
                MoveToStart(wordApp);

                //Заголовок программы
                document.Paragraphs[1].Range.Text = string.Format("{0}\r\n", app.Name);
                document.Paragraphs[1].Range.Font.Bold = 2;
                document.Paragraphs[1].Format.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                //1.1 Тип ППО
                WriteLineInFormular(wordApp, document, 4, app.Parent.Task.ToString());
                //1.2 Категория ППО
                WriteLineInFormular(wordApp, document, 5, app.Parent.Category.ToString());
                //1.3.2 Назначение каждого из звеньев
                WriteLineInFormular(wordApp, document, 8, app.Description);
                //1.3.3 Исполняемые среды каждого из звеньев
                WriteLineInFormular(wordApp, document, 9, app.Platform);
                //1.4.2 Назначение каждого из приложений
                WriteLineInFormular(wordApp, document, 13, app.Description);
                //2.1 Совместимые ОС с указанием версий
                WriteLineInFormular(wordApp, document, 16, app.CompatibleOSs);
                //2.2 Совместимые SCADA с указанием версий
                WriteLineInFormular(wordApp, document, 17, app.CompatibleScadas);
                //2.3 Совместимые СЗИ с указанием версий
                WriteLineInFormular(wordApp, document, 18, app.CompatibleSZI);
                //2.4 Другие необходимые типы ПО
                WriteLineInFormular(wordApp, document, 19, app.OtherSoft);
                //3.1 Тип идентификации и аутентификации
                WriteLineInFormular(wordApp, document, 21, app.IdentificationType);
                //3.2 Тип авторизации
                WriteLineInFormular(wordApp, document, 22, app.AuthorizationType);
                //3.3 Предполагаемые категории пользователей
                WriteLineInFormular(wordApp, document, 23, app.UserCategories);
                //4.1 Состав разделяемых и локально хранимых данных
                WriteLineInFormular(wordApp, document, 26, app.LocalData);
                //4.2 Используемая СУБД
                WriteLineInFormular(wordApp, document, 27, app.SUBD);
                //4.4 Используемые механизмы с средства хранения локальных данных
                WriteLineInFormular(wordApp, document, 29, app.DataStoringMechanism);
                //5.2 Компоненты и платформы, используемые для функционирования
                WriteLineInFormular(wordApp, document, 32, app.FunctionalComponents);
                //5.3 Компоненты и платформы, используемые для сборки
                WriteLineInFormular(wordApp, document, 33, app.BuildingComponents);
                //5.4 Средства предоставления отчетности
                WriteLineInFormular(wordApp, document, 34, app.Report);
                //6.2 Тип установщика
                WriteLineInFormular(wordApp, document, 36, app.Installer);
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
