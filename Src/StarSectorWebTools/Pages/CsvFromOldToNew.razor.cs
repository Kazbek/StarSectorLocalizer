using Common.Utils;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using StarSectorWebTools.Models.Dictionaries;
using StarSectorWebTools.Services;
using StarSectorWebTools.ViewModels;
using System.Data;

namespace StarSectorWebTools.Pages;

public partial class CsvFromOldToNew
{
    [Inject] FileSaverService FileSaverService { get; set; }

    private IBrowserFile OldOriginalFile { get; set; }
    private IBrowserFile OldTranslatedFile { get; set; }
    private IBrowserFile NewOriginalFile { get; set; }

    private DataTable DtOldOriginal { get; set; }
    private DataTable DtOldTranslated { get; set; }
    private DataTable DtNewOriginal { get; set; }

    private bool IsReadyToPrepare { get; set; }
    List<string> PreProcessErrors { get; set; } = new List<string>();
    ColumnsSelectViewModel ColumnsSelect { get; set; }

    bool IsReadyToProcess { get; set; }

    string ResultString = string.Empty;

    async Task SaveAsync()
    {
        ResultString = string.Empty;

        int missedKeys = 0;
        int translatedNew = 0;

        CsvOrderedDictionary dict = new CsvOrderedDictionary()
        {
            TranslatedColumns = ColumnsSelect.Columns.Where(t => t.ColumnType == ColumnType.ToTranslate).Select(t => t.Name).ToList()
        };

        CsvOrderedDictionary dictExtended = new CsvOrderedDictionary()
        {
            TranslatedColumns = ColumnsSelect.Columns.Where(t => t.ColumnType == ColumnType.ToTranslate).Select(t => t.Name).ToList()
        };

        int oldOroginalKeyIndex = DtOldOriginal.Columns.IndexOf(ColumnsSelect.Columns.Single(t => t.ColumnType == ColumnType.Key).Name);
        int oldTranslatedKeyIndex = DtOldTranslated.Columns.IndexOf(ColumnsSelect.Columns.Single(t => t.ColumnType == ColumnType.Key).Name);
        int newOroginalKeyIndex = DtNewOriginal.Columns.IndexOf(ColumnsSelect.Columns.Single(t => t.ColumnType == ColumnType.Key).Name);


        for (int i = 0; i < DtOldOriginal.Rows.Count; i++)
        {
            if (DtOldOriginal.Rows[i][oldOroginalKeyIndex].ToString() != DtOldTranslated.Rows[i][oldTranslatedKeyIndex].ToString())
            {
                missedKeys++;
                continue;
            }

            foreach (var cn in dict.TranslatedColumns)
            {
                int oldOroginalIndex = DtOldOriginal.Columns.IndexOf(cn);
                int oldTranslatedIndex = DtOldTranslated.Columns.IndexOf(cn);

                var orText = DtOldOriginal.Rows[i][oldOroginalIndex].ToString();
                var trText = DtOldTranslated.Rows[i][oldTranslatedIndex].ToString();
                if (orText != trText && !string.IsNullOrWhiteSpace(orText) && !string.IsNullOrWhiteSpace(trText))
                {
                    if(!dict.Translations.Contains(orText))
                        dict.Translations.Add(orText, trText);
                }
            }
        }

        for (int i = 0; i < DtNewOriginal.Rows.Count; i++)
        {
            foreach (var cn in dictExtended.TranslatedColumns)
            {
                int newOroginalIndex = DtNewOriginal.Columns.IndexOf(cn);

                var orText = DtOldOriginal.Rows[i][newOroginalIndex].ToString();
                if (string.IsNullOrWhiteSpace(orText))
                    continue;
                
                string trText = dict.Translations.Contains(orText) ? (string)dict.Translations[orText] : null;

                if (!dictExtended.Translations.Contains(orText))
                {
                    dictExtended.Translations.Add(orText, trText);
                    translatedNew++;
                }

            }
        }

        string json = JsonUtil.Serialize(dictExtended);
        byte[] data = System.Text.Encoding.UTF8.GetBytes(json);
        var fileStream = new MemoryStream(data);

        ResultString = $"Не совпали ключи по порядку в старых файлах: {missedKeys}. Переведено строк в новом файле: {translatedNew}";
        await FileSaverService.SaveAsFileAsync(NewOriginalFile.Name + ".translation.json", fileStream);
    }

    async Task<DataTable> ReadCsvAsync(IBrowserFile file)
    {
        using Stream fileStream = file.OpenReadStream(50 * 1024 * 1024);
        using var ms = new MemoryStream();
        await fileStream.CopyToAsync(ms);
        fileStream.Close();
        ms.Position = 0;
        return CsvUtils.Read(ms);
    }

    async Task PreProcessFilesAsync()
    {
        IsReadyToProcess = false;
        PreProcessErrors = new List<string>();
        ColumnsSelect = null;

        try
        {
            DtOldOriginal = await ReadCsvAsync(OldOriginalFile);
        }
        catch (Exception ex)
        {
            PreProcessErrors.Add($"Ошибка разбора старого непереведенного файла.");
            Console.WriteLine(ex.Message);
            Console.WriteLine(ex.StackTrace);
        }

        try
        {
            DtOldTranslated = await ReadCsvAsync(OldTranslatedFile);
        }
        catch (Exception ex)
        {
            PreProcessErrors.Add($"Ошибка разбора старого переведенного файла.");
            Console.WriteLine(ex.Message);
            Console.WriteLine(ex.StackTrace);
        }

        try
        {
            DtNewOriginal = await ReadCsvAsync(NewOriginalFile);
        }
        catch (Exception ex)
        {
            PreProcessErrors.Add($"Ошибка разбора нового непереведенного файла.");
            Console.WriteLine(ex.Message);
            Console.WriteLine(ex.StackTrace);
        }

        if (DtOldOriginal == null || DtOldTranslated == null || DtNewOriginal == null)
            return;

        if (DtOldOriginal.Rows.Count != DtOldTranslated.Rows.Count)
            PreProcessErrors.Add($"В старом непереведенном файле ({DtOldOriginal.Rows.Count}) и старом переведенном ({DtOldTranslated.Rows.Count}) не совпадает колиество строк.");

        if (DtOldOriginal.Columns.Count != DtOldTranslated.Columns.Count)
            PreProcessErrors.Add($"В старом непереведенном файле ({DtOldOriginal.Columns.Count}) и старом переведенном ({DtOldTranslated.Columns.Count}) не совпадает колиество столбцов.");

        List<string> columns = new List<string>();
        foreach(var c in DtNewOriginal.Columns.Cast<DataColumn>())
        {
            string name = c.ColumnName;
            if(DtOldOriginal.Columns.Contains(name) && DtOldTranslated.Columns.Contains(name))
                columns.Add(name);
        }

        if(columns.Count < 2)
        {
            PreProcessErrors.Add($"Недостаточно общих столбцов в файле, их всего {columns.Count}: {string.Join(", ", columns)}");
        }

        if (PreProcessErrors.Count > 0)
        {
            IsReadyToProcess = false;
            return;
        }

        ColumnsSelect = new ColumnsSelectViewModel{ Columns = columns.Select(t => new ColumnsSelectEntryViewModel { Name = t, ColumnType = ColumnType.Ignore }).ToList() };
        IsReadyToProcess = true;
    }

    private void SetOldOriginalFile(InputFileChangeEventArgs e)
    {
        OldOriginalFile = e?.File;
        CheckReadyToPrepare();
    }
    private void SetOldTranslatedFile(InputFileChangeEventArgs e)
    {
        OldTranslatedFile = e?.File;
        CheckReadyToPrepare();
    }

    private void SetNewOriginalFile(InputFileChangeEventArgs e)
    {
        NewOriginalFile = e?.File;
        CheckReadyToPrepare();
    }

    private void CheckReadyToPrepare()
    {
        PreProcessErrors = new List<string>();
        DtOldOriginal = null;
        DtOldTranslated = null;
        DtNewOriginal = null;
        IsReadyToPrepare = OldOriginalFile != null && OldTranslatedFile != null && NewOriginalFile != null;
        IsReadyToProcess = false;
        ColumnsSelect = null;
    }
}
