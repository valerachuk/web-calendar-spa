using AutoMapper;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Net.Http.Headers;
using WebCalendar.Business.Domains.Interfaces;
using WebCalendar.Business.Exceptions;
using WebCalendar.Business.ViewModels;
using WebCalendar.Data.Entities;
using WebCalendar.Data.Repositories.Interfaces;

namespace WebCalendar.Business.Domains
{
  public class FileDomain : IFileDomain
  {
    private readonly IFileRepository _fileRepository;
    private readonly IMapper _mapper;
    private readonly string STORAGE_PATH = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).ToString(), "EventFilesStorage\\");
    private const int MAX_FILE_SIZE = 10485760;

    public FileDomain(IFileRepository fileRepository, IMapper mapper)
    {
      _fileRepository = fileRepository;
      _mapper = mapper;
      
      if (!Directory.Exists(STORAGE_PATH))
      {
        Directory.CreateDirectory(STORAGE_PATH);
      }
    }
 
    public FileViewModel GetEventFile(int eventId) =>
      _mapper.Map<UserFile, FileViewModel>(_fileRepository.GetEventFile(eventId));

    public void DeleteFile(int fileId)
    {
      var userFile = _fileRepository.GetFile(fileId);
      if (userFile == null)
        return;

      if(File.Exists(userFile.Path))
        File.Delete(userFile.Path);

      _fileRepository.DeleteFile(userFile);
    }


    public int AddFile(IFormFile file)
    {
      if (file.Length == 0 || file.Length > MAX_FILE_SIZE)
        throw new FileSizeException("Incorrect file size");

      string fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');

      string uniqueFileName = Guid.NewGuid().ToString();
      string fullPath = Path.Combine(STORAGE_PATH, uniqueFileName);

      SaveFileToLocalStorage(file, fullPath);

      var userFile = new UserFile
      {
        Id = 0,
        Name = fileName,
        UniqueName = uniqueFileName,
        Path = fullPath,
        Size = file.Length,
        UploadDate = DateTime.Now,
        Type = file.ContentType
      };

      return _fileRepository.AddFile(userFile);
    }

    private void SaveFileToLocalStorage(IFormFile file, string fullPath)
    {
      using (var stream = new FileStream(fullPath, FileMode.Create))
      {
        file.CopyTo(stream);
      }
    }
  }
}
