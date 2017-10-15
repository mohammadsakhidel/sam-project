package com.ramanco.samandroid.api.endpoints;

import com.ramanco.samandroid.api.dtos.AppVersionDto;
import com.ramanco.samandroid.api.dtos.MosqueDto;
import com.ramanco.samandroid.consts.ApiActions;

import retrofit2.Call;
import retrofit2.http.GET;
import retrofit2.http.Query;

public interface SyncApiEndpoint {

    @GET(ApiActions.sync_appversion)
    Call<AppVersionDto> appVersion();

}
