import { Component, Input, OnChanges, OnInit, SimpleChanges } from '@angular/core';
import { ExecutedTask, IAutomaticTask, TaskStatus } from '../automatic-tasks/automatic-tasks.model';

const ELEMENT_DATA: ExecutedTask[] = [
  { executedDate: new Date(2022, 1, 1, 13, 25), status: TaskStatus.Succeeded },
  { executedDate: new Date(2022, 2, 1, 5, 55), status: TaskStatus.Succeeded },
  { executedDate: new Date(2022, 3, 1, 4, 15), status: TaskStatus.Failed },
  { executedDate: new Date(2022, 3, 1, 8, 5), status: TaskStatus.Created }
];

@Component({
  selector: 'app-executed-tasks',
  templateUrl: './executed-tasks.component.html',
  styleUrls: ['./executed-tasks.component.css']
})
export class ExecutedTasksComponent implements OnChanges{
  
  @Input()
  selectedTask!: IAutomaticTask
  
  public displayedColumns: string[] = ['executedDate', 'status'];
  public dataSource = ELEMENT_DATA;
  
  ngOnInit(): void {
  }
  
  ngOnChanges(changes: SimpleChanges): void {
    console.log('Executed Task changes: ', changes);
  }
}
