import { Location } from 'history'
import * as React from 'react'
import * as Navigator from '@framework/Navigator'
import * as AppContext from '@framework/AppContext'
import * as Finder from '@framework/Finder'
import { FindOptions, ValueSearchControl } from '@framework/Search'
import { Lite, liteKey } from '@framework/Signum.Entities'
import { ToolbarConfig, ToolbarResponse } from '../Toolbar/ToolbarClient'
import * as UserQueryClient from './UserQueryClient'
import { UserQueryEntity } from './Signum.Entities.UserQueries'
import { coalesceIcon } from '@framework/Operations/ContextualOperations';
import { useAPI } from '@framework/Hooks';
import { CountIcon } from '../Toolbar/QueryToolbarConfig';
import { useFetchInState } from '@framework/Navigator'
import { parseIcon } from '../Basics/Templates/IconTypeahead'

export default class UserQueryToolbarConfig extends ToolbarConfig<UserQueryEntity> {
  constructor() {
    var type = UserQueryEntity;
    super(type);
  }

  getIcon(element: ToolbarResponse<UserQueryEntity>) {

    if (element.iconName == "count")
      return <CountUserQueryIcon userQuery={element.content!} color={element.iconColor ?? "red"} autoRefreshPeriod={element.autoRefreshPeriod} />;

    return ToolbarConfig.coloredIcon(coalesceIcon(parseIcon(element.iconName), ["far", "list-alt"]), element.iconColor ?? "dodgerblue");
  }


  navigateTo(res: ToolbarResponse<UserQueryEntity>): Promise<string> {
    return Promise.resolve(UserQueryClient.userQueryUrl(res.content!));
  }

  isCompatibleWithUrl(res: ToolbarResponse<UserQueryEntity>, location: Location, query: any): boolean {
    return location.pathname == AppContext.toAbsoluteUrl(UserQueryClient.userQueryUrl(res.content!));
  }
}


interface CountUserQueryIconProps {
  userQuery: Lite<UserQueryEntity>;
  color?: string;
  autoRefreshPeriod?: number;
}


export function CountUserQueryIcon(p: CountUserQueryIconProps) {

  var userQuery = useFetchInState(p.userQuery)
  var findOptions = useAPI(signal => userQuery ? UserQueryClient.Converter.toFindOptions(userQuery, undefined) : Promise.resolve(undefined), [userQuery]);

  if (findOptions == null)
    return <span className="icon" style={{ color: p.color }}>…</span>;

  return <CountIcon findOptions={findOptions} autoRefreshPeriod={p.autoRefreshPeriod} color={p.color} />
}
