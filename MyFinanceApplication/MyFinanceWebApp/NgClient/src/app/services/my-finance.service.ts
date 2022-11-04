import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { GlobalVariables } from '../global-variables';
import { map } from 'rxjs/operators';
import { Observable } from 'rxjs';
import { BasicOption, UserSelectAccount } from '../automatic-tasks/automatic-tasks.model';

@Injectable({
  providedIn: 'root'
})
export class MyFinanceService {

  private baseUrl!: string;

  constructor(private globalVariables: GlobalVariables, private http: HttpClient) {
    this.baseUrl = globalVariables.baseUrl;
  }

  getDestinationAccounts(accountPeriodId: number, currencyId: number) {
    const url = `${this.baseUrl}/GetPossibleDestinationAccountAsync`;
    const params = new HttpParams().set('accountPeriodId', accountPeriodId).set('currencyId', currencyId);
    return this.http.get(url, { params: params })
      .pipe(map((response: any) => response));
  }

  getUserAccounts(): Observable<UserSelectAccount[]> {
    const url = `${this.baseUrl}/GetUserAccountsAsync`;
    console.log('Req url', url);
    return this.http.get(url)
      .pipe(map((responses: any) => responses));
  }

  getAddSpendData(accountPeriodId: number) {
    const params = new HttpParams().set('accountPeriodId', accountPeriodId);
    const url = `${this.baseUrl}/GetAddSpendViewModelAsync`;
    return this.http.get(url, { params: params })
      .pipe(map((response: any) => response));
  }
}
