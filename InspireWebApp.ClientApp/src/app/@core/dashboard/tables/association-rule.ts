export class DashboardTableAssociationRule implements IDashboardTableAssociationRule {
  rule!: string;
  confidence!: number;
  support!: number;
  lift!: number;

  constructor(data?: IDashboardTableAssociationRule) {
    if (data) {
      for (var property in data) {
        if (data.hasOwnProperty(property))
          (<any>this)[property] = (<any>data)[property];
      }
    }
  }

  init(_data?: any) {
    if (_data) {
      this.rule = _data["rule"] !== undefined ? _data["rule"] : <any>null;
      this.confidence = _data["confidence"] !== undefined ? _data["confidence"] : <any>null;
      this.support = _data["support"] !== undefined ? _data["support"] : <any>null;
      this.lift = _data["lift"] !== undefined ? _data["lift"] : <any>null;
    }
  }

  static fromJS(data: any): DashboardTableAssociationRule {
    data = typeof data === 'object' ? data : {};
    let result = new DashboardTableAssociationRule();
    result.init(data);
    return result;
  }

  toJSON(data?: any) {
    data = typeof data === 'object' ? data : {};
    data["rule"] = this.rule !== undefined ? this.rule : <any>null;
    data["confidence"] = this.confidence !== undefined ? this.confidence : <any>null;
    data["support"] = this.support !== undefined ? this.support : <any>null;
    data["lift"] = this.lift !== undefined ? this.lift : <any>null;
    return data;
  }
}

export interface IDashboardTableAssociationRule {
  rule: string;
  confidence: number;
  support: number;
  lift: number;
}
