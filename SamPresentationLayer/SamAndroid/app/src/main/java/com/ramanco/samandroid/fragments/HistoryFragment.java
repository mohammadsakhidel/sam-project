package com.ramanco.samandroid.fragments;


import android.app.ProgressDialog;
import android.os.Bundle;
import android.support.v4.app.Fragment;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.AdapterView;
import android.widget.ListView;
import android.widget.Toast;

import com.ramanco.samandroid.R;
import com.ramanco.samandroid.activities.MainActivity;
import com.ramanco.samandroid.adapters.ConsolationAdapter;
import com.ramanco.samandroid.api.dtos.ConsolationDto;
import com.ramanco.samandroid.api.dtos.MosqueDto;
import com.ramanco.samandroid.api.endpoints.ConsolationsApiEndpoint;
import com.ramanco.samandroid.api.endpoints.MosquesApiEndpoint;
import com.ramanco.samandroid.enums.PaymentStatus;
import com.ramanco.samandroid.exceptions.CallServerException;
import com.ramanco.samandroid.objects.KeyValuePair;
import com.ramanco.samandroid.utils.ApiUtil;
import com.ramanco.samandroid.utils.ExceptionManager;
import com.ramanco.samandroid.utils.PrefUtil;
import com.ramanco.samandroid.utils.TextUtility;
import com.ramanco.samandroid.utils.UxUtil;

import java.io.IOException;
import java.util.List;

import retrofit2.Response;

public class HistoryFragment extends Fragment {

    //region Ctors:
    public HistoryFragment() {
    }
    //endregion

    //region Overrides:
    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container,
                             Bundle savedInstanceState) {
        View v = inflater.inflate(R.layout.fragment_history, container, false);

        try {
            List<String> trackingNumbers = PrefUtil.getTrackingNumbers(getActivity());
            loadConsolationsAsync(trackingNumbers);
        } catch (Exception ex) {
            ExceptionManager.handle(getActivity(), ex);
        }

        return v;
    }
    //endregion

    //region Methods:
    private void loadConsolationsAsync(final List<String> trackNumbers) throws IOException, CallServerException {

        //region call server and fill list view:
        final ProgressDialog progress = UxUtil.showProgress(getActivity());
        new Thread(new Runnable() {
            @Override
            public void run() {
                try {
                    ConsolationsApiEndpoint endpoint = ApiUtil.createEndpoint(ConsolationsApiEndpoint.class);
                    String[] trackNumbersArray = trackNumbers.toArray(new String[trackNumbers.size()]);
                    Response<ConsolationDto[]> response = endpoint.find(trackNumbersArray).execute();
                    if (!response.isSuccessful())
                        throw new CallServerException(getActivity());
                    final ConsolationDto[] consolations = response.body();
                    //region fill list view:
                    getActivity().runOnUiThread(new Runnable() {
                        @Override
                        public void run() {
                            try {
                                View fragmentView = getView();
                                if (fragmentView != null) {
                                    ConsolationAdapter adapter = new ConsolationAdapter(getActivity(), consolations);
                                    ListView lvItems = (ListView) fragmentView.findViewById(R.id.lv_items);
                                    lvItems.setAdapter(adapter);
                                    lvItems.setOnItemClickListener(new AdapterView.OnItemClickListener() {
                                        @Override
                                        public void onItemClick(AdapterView<?> parent, View view, int position, long id) {
                                            try {
                                                ConsolationDto selectedConsolation = (ConsolationDto) parent.getItemAtPosition(position);
                                                PaymentStatus paymentStatus = PaymentStatus.valueOf(selectedConsolation.getPaymentStatus());
                                                if (paymentStatus == PaymentStatus.pending || paymentStatus == PaymentStatus.failed) {
                                                    showSendConsolationFragment(selectedConsolation);
                                                }
                                            } catch (Exception ex) {
                                                ExceptionManager.handle(getActivity(), ex);
                                            }
                                        }
                                    });
                                    progress.dismiss();
                                }
                            } catch (Exception ex) {
                                progress.dismiss();
                                ExceptionManager.handle(getActivity(), ex);
                            }
                        }
                    });
                    //endregion
                } catch (Exception ex) {
                    progress.dismiss();
                    ExceptionManager.handle(getActivity(), ex);
                }
            }
        }).start();
        //endregion

    }
    private void showSendConsolationFragment(ConsolationDto dto) {
        MainActivity mainActivity = (MainActivity) getActivity();

        SendConsolationFragment fragment = new SendConsolationFragment();
        fragment.setSelectedObit(dto.getObit());
        fragment.setSelectedTemplate(dto.getTemplate());
        fragment.setTemplateInfo(dto.getTemplateInfo());
        fragment.setCreatedConsolationId(dto.getId());
        fragment.setLoadFromPreview(true);

        mainActivity.replaceFragment(fragment);
        mainActivity.setBottomNavigationIndex(0);
    }
    //endregion
}