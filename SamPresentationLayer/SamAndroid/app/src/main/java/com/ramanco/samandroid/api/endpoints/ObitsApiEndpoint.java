package com.ramanco.samandroid.api.endpoints;

import com.ramanco.samandroid.api.dtos.ObitDto;
import com.ramanco.samandroid.consts.ApiActions;

import retrofit2.Call;
import retrofit2.http.GET;
import retrofit2.http.Query;

public interface ObitsApiEndpoint {

    @GET(ApiActions.obits_search)
    Call<ObitDto[]> search(@Query("query") String query);

    @GET(ApiActions.obits_getallobits)
    Call<ObitDto[]> getAllObits(@Query("mosqueId") int mosqueId);

    @GET(ApiActions.obits_gethenceforwardobits)
    Call<ObitDto[]> getHenceForwardObits(@Query("mosqueId") int mosqueId);

}
