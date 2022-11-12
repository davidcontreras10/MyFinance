import { Component, Input, OnChanges, OnInit, SimpleChanges } from '@angular/core';
import { ExecutedTask, IAutomaticTask, TaskStatus } from '../automatic-tasks/automatic-tasks.model';
import { MyFinanceService } from '../services/my-finance.service';

@Component({
  selector: 'app-executed-tasks',
  templateUrl: './executed-tasks.component.html',
  styleUrls: ['./executed-tasks.component.css']
})
export class ExecutedTasksComponent implements OnChanges {

  constructor(private service: MyFinanceService) { }

  @Input()
  selectedTask!: IAutomaticTask

  public displayedColumns: string[] = ['executedDate', 'status', 'message'];
  public dataSource!: ExecutedTask[];

  ngOnInit(): void {
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (this.selectedTask) {
      this.service.getExecutedTasks(this.selectedTask.id).subscribe({
        next: this._executedTasksReceived.bind(this),
        error: this._onServiceError.bind(this)
      })
    }
  }

  private _executedTasksReceived(tasks: ExecutedTask[]) {
    this.dataSource = tasks;
  }

  private _onServiceError(error: any) {
    console.error(error);
  }
}
