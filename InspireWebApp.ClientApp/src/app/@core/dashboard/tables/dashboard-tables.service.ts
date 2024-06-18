import {Inject, Injectable} from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {API_BASE} from "../../../app-constants";
import {Observable, map} from "rxjs";
import {DashboardTableAssociationRule} from "./association-rule";

@Injectable({
  providedIn: 'root'
})
export class DashboardTablesService {
  constructor(
    private readonly http: HttpClient,
    @Inject(API_BASE) private readonly apiBase: string,
  ) {
  }

  private readonly resourceBase = this.apiBase + 'dashboard-tables/';

  public getAssociationRules(): Observable<DashboardTableAssociationRule[]> {
    return this.http.get<any[]>(this.resourceBase + 'association-rules')
      .pipe(
        map(apiResponse => apiResponse.map(record => DashboardTableAssociationRule.fromJS(record))),
      );
  }
}
