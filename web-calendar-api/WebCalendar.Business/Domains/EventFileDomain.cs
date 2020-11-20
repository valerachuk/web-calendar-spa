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
  public class EventFileDomain : IEventFileDomain
  {
    private readonly IEventFileRepository _efRepository;
    private readonly IEventRepository _eventRepository;
    private readonly IMapper _mapper;
    private readonly string STORAGE_PATH = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).ToString(), "EventFilesStorage\\");
    private const int MAX_FILE_SIZE = 10485760;

    public EventFileDomain(IEventFileRepository fileRepository, IEventRepository eventRepository, IMapper mapper)
    {
      _efRepository = fileRepository;
      _eventRepository = eventRepository;
      _mapper = mapper;
      
      if (!Directory.Exists(STORAGE_PATH))
      {
        Directory.CreateDirectory(STORAGE_PATH);
      }
    }
 
    public EventFileViewModel GetEventFile(int eventId) =>
      _mapper.Map<EventFile, EventFileViewModel>(_efRepository.GetEventFile(eventId));

    public void DeleteEventFile(int eventId)
    {
      if (_eventRepository.GetEvent(eventId) == null)
        throw new NotFoundException("Event not found");

      var eventFile = GetEventFile(eventId);
      if (eventFile == null)
        return;

      if(File.Exists(eventFile.Path))
        File.Delete(eventFile.Path);
    }


    public int AddFile(IFormFile file, int eventId)
    {
      if (_eventRepository.GetEvent(eventId) == null)
        throw new NotFoundException("Event not found");

      if (GetEventFile(eventId) != null)
        throw new ForbiddenException("This event already has an attached file");

      if (file.Length == 0 || file.Length > MAX_FILE_SIZE)
        throw new FileSizeException("Incorrect file size");

      string fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');

      string uniqueFileName = Guid.NewGuid().ToString();
      string fullPath = Path.Combine(STORAGE_PATH, uniqueFileName);

      SaveFileToLocalStorage(file, fullPath);

      var evFile = new EventFile
      {
        Id = 0,
        Name = fileName,
        UniqueName = uniqueFileName,
        Path = fullPath,
        Size = file.Length,
        UploadDate = DateTime.Now,
        Type = file.ContentType,
        EventId = eventId
      };

      return _efRepository.AddEventFile(evFile);
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
