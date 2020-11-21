import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FileSystemFileEntry, NgxFileDropEntry } from 'ngx-file-drop';
import { saveAs } from 'file-saver';
import { FileAttachService } from 'src/app/services/file-attach.service';
import { ToastGlobalService } from 'src/app/services/toast-global.service';
import { throwError } from 'rxjs';

@Component({
  selector: 'app-file-attach',
  templateUrl: './file-attach.component.html',
  styleUrls: ['./file-attach.component.css']
})
export class FileAttachComponent implements OnInit {
  @Input() eventId: number;
  @Input() public attachedFile: File;
  @Output() attachedFileChange = new EventEmitter<File>();

  public downloadFile: File = null;

  private readonly maxFileSize = 10485760;

  constructor(
    private toastService: ToastGlobalService,
    private fileService: FileAttachService
  ) { }

  ngOnInit(): void {
    this.attachedFileInit();
  }

  public dropped(newFiles: NgxFileDropEntry[]) {
    for (const droppedFile of newFiles) {
      if (droppedFile.fileEntry.isFile) {
        const fileEntry = droppedFile.fileEntry as FileSystemFileEntry;
        fileEntry.file((file: File) => {
          if(file.size > this.maxFileSize)
          {
            this.toastService.add({
              delay: 5000,
              title: 'Warning!',
              content: `File ${file.name} is too long`,
              className: "bg-warning text-light"
            });
            return;
          }
          this.attachedFile = file;
          this.attachedFileChange.emit(this.attachedFile);
        });
        break;
      }
    }
  }

  attachedFileInit() {
    if(this.eventId === null || this.eventId === undefined)
      return;
    
    this.fileService.getEventFile(this.eventId).subscribe(response => {
      this.downloadFile = new File([response.body], this.getFileName(response.headers), {type: response.body.type});
    }, err => {
      if(err.status === 404)
        this.downloadFile = null;
      if(err.status === 415)
        this.toastService.add({
          delay: 5000,
          title: 'Warning!',
          content: `File is too long`,
          className: "bg-warning text-light"
        });
      else
        throwError(err);
    });
  }

  getFileName(headers) {
    let contentDispositionHeader = headers.get('content-disposition');
    let result = contentDispositionHeader.split(';')[1].trim().split('=')[1];
    return result.replace(/"/g, '');
  }

  download() {
    saveAs(this.downloadFile);
  }
  
}
