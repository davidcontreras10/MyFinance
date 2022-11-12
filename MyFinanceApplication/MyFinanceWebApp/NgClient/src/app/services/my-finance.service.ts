import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { GlobalVariables } from '../global-variables';
import { map } from 'rxjs/operators';
import { Observable } from 'rxjs';
import { BasicOption, ExecutedTask, IAutomaticTask, UserSelectAccount } from '../automatic-tasks/automatic-tasks.model';
import { BasicNewScheduledTask, TransferNewScheduledTask } from './models';

@Injectable({
  providedIn: 'root'
})
export class MyFinanceService {

  private baseUrl!: string;

  constructor(globalVariables: GlobalVariables, private http: HttpClient) {
    this.baseUrl = globalVariables.baseUrl;
  }

  deleteScheduledTask(taskId: string) {
    const url = `${this.baseUrl}/DeleteScheduledTaskAsync`;
    const params = new HttpParams().set('taskId', taskId);
    return this.http.delete(url, { params: params });
  }

  getScheduledTasks(): Observable<IAutomaticTask[]> {
    const url = `${this.baseUrl}/GetScheduledTasksAsync`;
    return this.http.get(url).pipe(map((data: any) => this._mapScheduledTasks(data)));
  }

  getDestinationAccounts(accountPeriodId: number, currencyId: number) {
    const url = `${this.baseUrl}/GetPossibleDestinationAccountAsync`;
    const params = new HttpParams().set('accountPeriodId', accountPeriodId).set('currencyId', currencyId);
    return this.http.get(url, { params: params })
      .pipe(map((response: any) => response));
  }

  getUserAccounts(): Observable<UserSelectAccount[]> {
    const url = `${this.baseUrl}/GetUserAccountsAsync`;
    return this.http.get(url)
      .pipe(map((responses: any) => responses));
  }

  getAddSpendData(accountPeriodId: number) {
    const params = new HttpParams().set('accountPeriodId', accountPeriodId);
    const url = `${this.baseUrl}/GetAddSpendViewModelAsync`;
    return this.http.get(url, { params: params })
      .pipe(map((response: any) => response));
  }

  createBasic(model: BasicNewScheduledTask) {
    const url = `${this.baseUrl}/CreateBasicAsync`;
    return this.http.post(url, model);
  }

  createTransfer(model: TransferNewScheduledTask) {
    const url = `${this.baseUrl}/CreateTransferAsync`;
    return this.http.post(url, model);
  }

  getExecutedTasks(taskId: string): Observable<ExecutedTask[]> {
    const url = `${this.baseUrl}/GetExecutedTaskAsync`;
    const params = new HttpParams().set('taskId', taskId);
    return this.http.get(url, { params: params })
      .pipe(map((responses: any) => responses));;
  }

  private _mapScheduledTasks(tasks: IAutomaticTask[]): IAutomaticTask[] {
    return tasks;
  }
}
