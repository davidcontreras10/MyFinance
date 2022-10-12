import { Component, Input, OnChanges, OnInit, SimpleChanges } from '@angular/core';
import { IAutomaticTask } from '../automatic-tasks/automatic-tasks.model';

@Component({
  selector: 'app-task-detail',
  templateUrl: './task-detail.component.html',
  styleUrls: ['./task-detail.component.css']
})
export class TaskDetailComponent implements OnInit, OnChanges {

  @Input()
  selectedTask!: IAutomaticTask

  public showRecords: boolean = false;

  private viewRecordsId: string = "";

  ngOnInit(): void {
  }

  ngOnChanges(changes: SimpleChanges): void {
    this.showRecords = !!this.selectedTask?.id && !!this.viewRecordsId && this.viewRecordsId === this.selectedTask.id;
  }

  public onViewRecordsClicked():void{
    console.log('view records clicked');
    if(this.selectedTask?.id){
      this.showRecords = true;
      this.viewRecordsId = this.selectedTask.id;
    } else{
      this.showRecords = false;
      this.viewRecordsId = "";
    }
  }
}
