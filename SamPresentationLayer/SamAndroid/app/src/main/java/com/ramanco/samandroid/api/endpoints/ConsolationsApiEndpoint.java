package com.ramanco.samandroid.api.endpoints;

import com.ramanco.samandroid.api.dtos.ConsolationDto;
import com.ramanco.samandroid.consts.ApiActions;

import java.util.Map;

import retrofit2.Call;
import retrofit2.http.Body;
import retrofit2.http.GET;
import retrofit2.http.POST;
import retrofit2.http.PUT;
import retrofit2.http.Path;
import retrofit2.http.Query;

public interface ConsolationsApiEndpoint {

    @POST(ApiActions.consolations_create)
    Call<Map<String, Object>> create(@Body ConsolationDto dto);

    @POST(ApiActions.consolations_find)
    Call<ConsolationDto[]> find(@Body String[] trackingNumbers);

    @PUT(ApiActions.consolations_update + "/{id}")
    Call<Void> update(@Path("id") int id, @Body Map<String, String> fields,
                      @Query("newstatus") String newStatus, @Query("obitid") int obitId,
                      @Query("templateid") int templateId);

}


