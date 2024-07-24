export class DashboardTableSegmentDetails implements IDashboardTableSegmentDetails {
  segment!: string;
  description!: string;
  typicalActions!: string;

  constructor(data?: IDashboardTableSegmentDetails) {
    if (data) {
      for (var property in data) {
        if (data.hasOwnProperty(property))
          (<any>this)[property] = (<any>data)[property];
      }
    }
  }

  init(_data?: any) {
    if (_data) {
      this.segment = _data["segment"] !== undefined ? _data["segment"] : <any>null;
      this.description = _data["description"] !== undefined ? _data["description"] : <any>null;
      this.typicalActions = _data["typicalActions"] !== undefined ? _data["typicalActions"] : <any>null;
    }
  }

  static fromJS(data: any): DashboardTableSegmentDetails {
    data = typeof data === 'object' ? data : {};
    let result = new DashboardTableSegmentDetails();
    result.init(data);
    return result;
  }

  toJSON(data?: any) {
    data = typeof data === 'object' ? data : {};
    data["segment"] = this.segment !== undefined ? this.segment : <any>null;
    data["description"] = this.description !== undefined ? this.description : <any>null;
    data["typicalActions"] = this.typicalActions !== undefined ? this.typicalActions : <any>null;
    return data;
  }

  clone(): DashboardTableSegmentDetails {
    const json = this.toJSON();
    let result = new DashboardTableSegmentDetails();
    result.init(json);
    return result;
  }
}

export interface IDashboardTableSegmentDetails {
  segment: string;
  description: string;
  typicalActions: string;
}
