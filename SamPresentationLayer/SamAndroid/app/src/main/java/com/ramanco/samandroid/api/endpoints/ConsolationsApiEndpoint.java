package com.ramanco.samandroid.api.endpoints;

import com.ramanco.samandroid.api.dtos.ConsolationDto;
import com.ramanco.samandroid.consts.ApiActions;

import retrofit2.Call;
import retrofit2.http.Body;
import retrofit2.http.GET;
import retrofit2.http.POST;

public interface ConsolationsApiEndpoint {

    @POST(ApiActions.consolations_create)
    Call<Integer> create(@Body ConsolationDto dto);

}


