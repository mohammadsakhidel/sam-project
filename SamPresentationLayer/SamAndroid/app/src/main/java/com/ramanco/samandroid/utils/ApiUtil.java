package com.ramanco.samandroid.utils;

import com.ramanco.samandroid.consts.Configs;

import java.io.IOException;

import okhttp3.Interceptor;
import okhttp3.OkHttpClient;
import okhttp3.Request;
import okhttp3.Response;
import retrofit2.Retrofit;
import retrofit2.converter.gson.GsonConverterFactory;

public class ApiUtil {
    public static OkHttpClient getAuthHeaderAttacherHttpClient() {
        return new OkHttpClient.Builder().addInterceptor(
                new Interceptor() {
                    @Override
                    public Response intercept(Interceptor.Chain chain) throws IOException {
                        Request request = chain.request().newBuilder()
                                //.addHeader("Authorization", Configs.API_BASIC_AUTH_HEADER_SCHEME_PARAM)
                                .build();
                        return chain.proceed(request);
                    }
                }).build();
    }

    public static <E> E createEndpoint(Class<E> clazz) {

        Retrofit retrofit = new Retrofit.Builder()
                .baseUrl(Configs.API_BASE_ADDRESS)
                .client(getAuthHeaderAttacherHttpClient())
                .addConverterFactory(GsonConverterFactory.create())
                .build();

        return  retrofit.create(clazz);

    }
}
